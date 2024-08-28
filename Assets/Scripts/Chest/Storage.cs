using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Storage : NetworkBehaviour
{
    [SyncVar]
    private bool isOpened;

    public string name;
    public int size;
    public SyncList<StorageItem> items = new SyncList<StorageItem>();

    [SerializeField] RandomSpawningItems itemsToSpawn;
    [SerializeField] bool spawnItems;

    private bool itemsSpawned;

    [SerializeField] private GameObject upperChest;

    [SerializeField] private GameObject proximityPrompt;
    [SerializeField] private float promptDistance = 3f;

    public override void OnStartServer()
    {
        int itemsToAdd = size - items.Count;
        for (int i = 0; i < itemsToAdd; i++)
        {
            items.Add(new StorageItem(0, null));
        }

        if (spawnItems && !itemsSpawned)
        {
            int count = Random.Range(itemsToSpawn.minCount, itemsToSpawn.maxCount + 1);

            for (int i = 0; i < count; i++)
            {
                ItemSO itemToSpawn = itemsToSpawn.itemsToSpawn[Random.Range(0, itemsToSpawn.itemsToSpawn.Count)];

                items[i].itemScriptableObject = itemToSpawn;

                if (items[i].itemScriptableObject.stackMax > 1)
                {
                    int countToSpawn = Random.Range(1, items[i].itemScriptableObject.stackMax + 1);
                    items[i].currentStack = countToSpawn;
                }
                else
                {
                    items[i].currentStack = 1;
                }
            }
        }
    }

    void Update()
    {
        if (!isClient) return;

        if ((NetworkClient.localPlayer.transform.position - transform.position).magnitude > promptDistance)
        {
            proximityPrompt.SetActive(false);
        }
        else if (!isOpened)
        {
            proximityPrompt.SetActive(true);
        }

        Vector3 position = proximityPrompt.transform.position;
        Vector3 target = NetworkClient.localPlayer.gameObject.transform.position;
        Vector3 inverseHeight = new Vector3(0, (position.y - target.y) * 2, 0);
        proximityPrompt.transform.LookAt(2 * (position + inverseHeight) - target);
    }

    [Command(requiresAuthority = false)]
    public void CmdOpenChest()
    {
        Debug.Log("opening");

        if (isOpened) return;

        isOpened = true;
        proximityPrompt.SetActive(false);

        GetComponent<AudioSource>().Play();

        foreach (StorageItem item in items)
        {
            if (item != null && item.itemScriptableObject.prefab != null)
            {
                GameObject newWeapon = Instantiate(item.itemScriptableObject.prefab, transform.position + transform.forward * 3, transform.rotation);

                NetworkServer.Spawn(newWeapon);
            }
        }

        if (upperChest != null)
        {
            upperChest.transform.Rotate(-45, 0, 0);
        }
    }
}
