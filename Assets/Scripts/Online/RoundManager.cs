using Mirror;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private GameObject test;

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

    IEnumerator HandleRounds()
    {
        yield return new WaitForSeconds(10);

        foreach (KeyValuePair<int, NetworkConnectionToClient> entry in NetworkServer.connections)
        {
            GameObject newFolder = new GameObject("Map" + entry.Key.ToString());
            newFolder.transform.position = new Vector3(0, 0, entry.Key * mapSpacing + mapSpacing);
        }

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

                    Transform mapFolder = GameObject.Find("Map" + entry.Key.ToString()).transform;

                    foreach (Transform child in mapFolder)
                    {
                        GameObject.Destroy(child.gameObject);
                    }

                    GameObject NewMap = Instantiate(chosenMap.gameObject);
                    NewMap.transform.SetParent(mapFolder);
                    NewMap.transform.position = mapFolder.transform.position;

                    NetworkServer.Spawn(NewMap, conn);

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
}
