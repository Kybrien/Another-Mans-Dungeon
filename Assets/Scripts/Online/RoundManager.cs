using Mirror;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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

    [SerializeField] private int mapDistance = 10000;

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

        for (int i = 1; i <= NetworkManager.singleton.numPlayers; i++)
        {
            GameObject newFolder = new GameObject("Map" + i.ToString());
            newFolder.transform.position = new Vector3(0, 0, i * mapDistance);
        }

        while (currentRound < rounds)
        {
            if (secondsLeft > 0)
            {
                secondsLeft -= 1;
                Debug.Log("timer " + secondsLeft.ToString());
            } else
            {
                currentRound += 1;
                secondsLeft = maxRoundTimer;

                MapData chosenMap = SelectRandomMap();
                Debug.Log(chosenMap.name);

                Debug.Log("rpc sent");

                GameObject NewMap = Instantiate(chosenMap.gameObject);
                NewMap.transform.position = new Vector3(0, 0, 0);

                NetworkServer.Spawn(NewMap);

                RpcSwitchMap(NewMap);
            }

            yield return new WaitForSeconds(1);
        }
    }

    [ClientRpc]
    void RpcSwitchMap(GameObject map)
    {
        Transform mapFolder = GameObject.Find("Map" + NetworkClient.localPlayer.netId.ToString()).transform;

        foreach (Transform child in mapFolder)
        {
            GameObject.Destroy(child.gameObject);
        }

        Debug.Log("Round " + currentRound.ToString() + " started");

        map.transform.SetParent(mapFolder);
        map.transform.position = Vector3.zero;

        /*        GameObject NewMap = Instantiate(map.gameObject);
                NewMap.transform.position = new Vector3(0, 0, 0);
                NewMap.transform.SetParent(mapFolder);*/
    }
}
