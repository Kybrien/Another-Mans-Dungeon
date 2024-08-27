using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : NetworkBehaviour
{
    [SyncVar]
    private bool isOpened;

    public string name;
    public int size;
    public List<StorageItem> items = new List<StorageItem>();

    [SerializeField] RandomSpawningItems itemsToSpawn;
    [SerializeField] bool spawnItems;

    private bool itemsSpawned;

    [SerializeField] private GameObject worldUIGO;

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

    private void Update()
    {
        if (isServer) {
            Debug.Log("server");
        }

        Vector3 position = worldUIGO.transform.position;
        Vector3 target = NetworkClient.localPlayer.gameObject.transform.Find("CameraRoot").Find("CameraControls").Find("Camera").position;
        Vector3 inverseHeight = new Vector3(0, (position.y - target.y) * 2, 0);
        worldUIGO.transform.LookAt(2 * (position + inverseHeight) - target);
    }

    [Command(requiresAuthority = false)]
    public void CmdOpenChest()
    {
        if (isOpened) return;

        isOpened = true;

        GetComponent<AudioSource>().Play();

        foreach (StorageItem item in items)
        {
            if (item != null && item.itemScriptableObject.prefab != null)
            {
                GameObject newWeapon = Instantiate(item.itemScriptableObject.prefab, transform.position + transform.forward * 3, transform.rotation);

                NetworkServer.Spawn(newWeapon);
            }
        }

        Debug.Log("Opened");
    }
}
