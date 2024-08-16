using Mirror;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class RoundManager : NetworkBehaviour
{
    [Serializable]
    public class MapData
    {
        public string name;
        public GameObject gameObject;
    }

    [SerializeField] private GameObject mapFolderPrefab;

    [Header("GAME SETTINGS")]

    [SerializeField] private int rounds = 5;

    [SerializeField] private int maxRoundTimer = 30;

    [SerializeField] private List<MapData> maps;

    [SerializeField] private int mapSpacing = 10000;

    [Header("GAME PROGRESSION (DO NOT TOUCH)")]

    [SyncVar]
    private string currentMap = "";

    [SyncVar]
    private int currentRound = 0;

    [SyncVar]
    private int secondsLeft = 0;

    public readonly SyncDictionary<int, GameObject> playerMapFolders = new SyncDictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnStartServer()
    {
        Debug.Log("Start!");
        StartCoroutine(HandleRounds());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    MapData SelectRandomMap()
    {
        MapData chosenMap = maps[UnityEngine.Random.Range(0, maps.Count)];
        return chosenMap;
    }

    [Server]
    IEnumerator HandleRounds()
    {
        for (int i = 10; i > 5; i--)
        {
            secondsLeft = i;
            RpcUpdateStatus();
            yield return new WaitForSeconds(1);
        }

        foreach (KeyValuePair<int, NetworkConnectionToClient> entry in NetworkServer.connections)
        {
            NetworkConnectionToClient conn = entry.Value;

            GameObject newFolder = Instantiate(mapFolderPrefab);
            newFolder.transform.position = new Vector3(0, 0, entry.Key * mapSpacing + mapSpacing);
            //newFolder.name = "Map" + entry.Key.ToString();

            NetworkServer.Spawn(newFolder, conn);

            playerMapFolders.Add(entry.Key, newFolder);

            //RpcSetMapName(newFolder, "Map" + entry.Key.ToString());
        }

        for (int i = 5; i >= 1; i--)
        {
            secondsLeft = i;
            RpcUpdateStatus();
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(3);

        while (currentRound < rounds)
        {
            if (secondsLeft > 0)
            {
                secondsLeft -= 1;
                RpcUpdateStatus();
                Debug.Log("timer " + secondsLeft.ToString());
            } else
            {
                currentRound += 1;
                secondsLeft = maxRoundTimer;

                MapData chosenMap = SelectRandomMap();
                Debug.Log(chosenMap.name);

                foreach (KeyValuePair<int, NetworkConnectionToClient> entry in NetworkServer.connections)
                {
                    NetworkConnectionToClient conn = entry.Value;
                    GameObject player = conn.identity.gameObject;

                    //Transform mapFolder = GameObject.Find("Map" + entry.Key.ToString()).transform;
                    Transform mapFolder = playerMapFolders[entry.Key].transform;

                    foreach (Transform child in mapFolder)
                    {
                        GameObject.Destroy(child.gameObject);
                    }

                    GameObject NewMap = Instantiate(chosenMap.gameObject, mapFolder);
                    //NewMap.transform.SetParent(mapFolder);
                    NewMap.transform.position = mapFolder.transform.position;

                    NetworkServer.Spawn(NewMap);

                    //RpcSetMapParent(NewMap, mapFolder);

                    if (NewMap.transform.Find("SpawnLocation"))
                    {
                        player.transform.position = NewMap.transform.Find("SpawnLocation").transform.position;
                    } else
                    {
                        Debug.LogWarning("No SpawnLocation found!");
                        player.transform.position = NewMap.transform.position;
                    }

                    PlayerMovementController plrData = player.GetComponent<PlayerMovementController>();
                    plrData.SetHealth(plrData.GetMaxHealth());
                }

                Debug.Log("Round " + currentRound.ToString() + " started");

                RpcUpdateStatus();
            }

            yield return new WaitForSeconds(1);
        }
    }

    [ClientRpc]
    public void RpcUpdateStatus()
    {
        NetworkClient.localPlayer.GetComponent<PlayerMovementController>().UpdateStatus(currentRound, secondsLeft);
    }

    [ClientRpc]
    public void RpcSetMapName(GameObject map, string newName)
    {
        map.name = newName;
    }

    [ClientRpc]
    public void RpcSetMapParent(GameObject map, Transform parent)
    {
        map.transform.parent = parent;
    }
}
