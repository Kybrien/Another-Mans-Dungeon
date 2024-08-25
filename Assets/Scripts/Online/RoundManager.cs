using Mirror;
using Newtonsoft.Json.Linq;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using static RootMotion.FinalIK.GenericPoser;

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

    [SerializeField] private List<MapData> maps1v1;

    [SerializeField] private int mapSpacing = 10000;

    [Header("GAME PROGRESSION (DO NOT TOUCH)")]

    [SyncVar]
    private string currentMap = "";

    [SyncVar]
    private int currentRound = 0;

    [SyncVar]
    private int secondsLeft = 0;

    [SyncVar]
    private int playerThroughPortal = 0;

    [SyncVar]
    private int playersAlive = 0;

    public readonly SyncDictionary<int, GameObject> playerMapFolders = new SyncDictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnStartServer()
    {
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

    MapData SelectRandom1v1Map()
    {
        MapData chosenMap = maps1v1[UnityEngine.Random.Range(0, maps1v1.Count)];
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

        yield return new WaitForSeconds(1);

        while (currentRound <= rounds)
        {
            if (secondsLeft > 0 && playersAlive > 0)
            {
                if (((currentRound == 3 || currentRound == 5) && NetworkManager.singleton.numPlayers > 1 && playersAlive == 1) || playersAlive == 0)
                {
                    secondsLeft = 0;
                }
                else
                {
                    secondsLeft -= 1;
                }
                RpcUpdateStatus();
            }
            else
            {
                currentRound += 1;

                if (currentRound > rounds) break;

                playerThroughPortal = 0;
                playersAlive = 0;
                secondsLeft = maxRoundTimer;

                if ((currentRound == 3 || currentRound == 5) && NetworkManager.singleton.numPlayers > 1)
                {
                    MapData chosenMap = SelectRandom1v1Map();
                    RpcStartLoadingScreen(true, chosenMap.name);

                    Debug.Log("1V1 START");

                    yield return new WaitForSeconds(0.1f);

                    GameObject NewMap = Instantiate(chosenMap.gameObject);
                    NetworkServer.Spawn(NewMap);

                    foreach (KeyValuePair<int, NetworkConnectionToClient> entry in NetworkServer.connections)
                    {
                        NetworkConnectionToClient conn = entry.Value;
                        GameObject player = conn.identity.gameObject;

                        PlayerMovementController plrData = player.GetComponent<PlayerMovementController>();
                        plrData.SetHealth(plrData.GetMaxHealth());

                        string portalName = "Portal_Player" + (1 + entry.Key % 2).ToString();
                        Debug.Log("Player id: " + conn.identity.netId.ToString() + ". portal name = " + portalName);
                        //player.transform.position = NewMap.transform.Find(portalName).position;

                        TeleportToPortal(NetworkClient.localPlayer.transform, NewMap, portalName);

                        playersAlive += 1;
                    }
                }
                else {
                    MapData chosenMap = SelectRandomMap();
                    RpcStartLoadingScreen(true, chosenMap.name);

                    yield return new WaitForSeconds(0.1f);

                    foreach (KeyValuePair<int, NetworkConnectionToClient> entry in NetworkServer.connections)
                    {
                        NetworkConnectionToClient conn = entry.Value;
                        GameObject player = conn.identity.gameObject;

                        //Transform mapFolder = GameObject.Find("Map" + entry.Key.ToString()).transform;
                        Transform mapFolder = playerMapFolders[entry.Key].transform;

                        foreach (Transform child in mapFolder)
                        {
                            NetworkServer.Destroy(child.gameObject);
                            //GameObject.Destroy(child.gameObject);
                        }

                        GameObject NewMap = Instantiate(chosenMap.gameObject, mapFolder);
                        //NewMap.transform.SetParent(mapFolder);
                        //NewMap.transform.position = mapFolder.transform.position;

                        NetworkServer.Spawn(NewMap);

                        RpcSwitchMap(NewMap, mapFolder);
                        RpcTeleportToSpawn(conn, NewMap);

                        PlayerMovementController plrData = player.GetComponent<PlayerMovementController>();
                        plrData.SetHealth(plrData.GetMaxHealth());

                        playersAlive += 1;
                    }
                }

                RpcStartLoadingScreen(false, "");
                RpcUpdateStatus();

                playerThroughPortal = 0;
            }

            yield return new WaitForSeconds(1);
        }

        currentRound = -2;
        RpcUpdateStatus();

        EndGameAndReturnToLobby();
    }

    private void TeleportToPortal(Transform player, GameObject map, string name)
    {
        Transform PortalStart = map.transform.Find(name);
        if (PortalStart)
        {
            player.position = PortalStart.transform.position;
        }
        else
        {
            Debug.LogWarning("No SpawnLocation found!");
            player.position = map.transform.position;
        }
    }

    public void EndGameAndReturnToLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }

        NetworkManager.singleton.ServerChangeScene("LobbyScene");
    }

    [Command(requiresAuthority = false)]
    public void CmdInvadeWorld(GameObject portal, GameObject player)
    {
        Debug.Log("received portal request on server");
        playerThroughPortal += 1;

        Debug.Log(playerThroughPortal);
        Debug.Log(NetworkManager.singleton.numPlayers);

        if (playerThroughPortal == NetworkManager.singleton.numPlayers)
        {
            secondsLeft = 0;
        }
        else
        {
            int playerIdentity = (int)player.GetComponent<NetworkIdentity>().netId;

            foreach (KeyValuePair<int, NetworkConnectionToClient> entry in NetworkServer.connections)
            {
                if (playerIdentity != entry.Key)
                {
                    TeleportToPortal(NetworkClient.localPlayer.transform, playerMapFolders[entry.Key], "PortalStart");
                    RpcInvadeWorld(entry.Value);

                    break;
                }
            }
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
    public void RpcSwitchMap(GameObject map, Transform parent)
    {
        AstarPath astarPath = map.transform.Find("Astar").GetComponent<AstarPath>();
        Pathfinding.GridGraph graph = astarPath.data.gridGraph;
        Vector3 offset = new Vector3(0, 0, parent.transform.position.z);

        map.transform.parent = parent;
        if (isClient && !isServer)
        {
            map.transform.position += offset;

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (obj.transform.parent == map)
                {
                    obj.transform.position += offset;
                    Debug.Log(obj.name + " moved.");
                }
            }
        }

        graph.center += offset;

        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx =>
        {
            AstarPath.active.Scan(graph);
        }));
    }

    [TargetRpc]
    public void RpcTeleportToSpawn(NetworkConnectionToClient target, GameObject map)
    {
        if (map == null)
        {
            Debug.LogError("Map is null, cannot teleport!");
            return;
        }

        TeleportToPortal(NetworkClient.localPlayer.transform, map, "PortalStart");
    }

    [ClientRpc]
    public void RpcStartLoadingScreen(bool load, string mapName)
    {
        PlayerLoadingScreen playerLoadingScreen = NetworkClient.localPlayer.GetComponent<PlayerLoadingScreen>();
        if (load)
        {
            StartCoroutine(playerLoadingScreen.Load(mapName));
        }
        else
        {
            playerLoadingScreen.Complete();
        }
    }

    [TargetRpc]
    public void RpcInvadeWorld(NetworkConnectionToClient target)
    {
        Debug.Log("Your world is invaded !");
    }

    [Server]
    public void PlayerDied()
    {
        if (isServer)
        {
            playersAlive -= 1;
            Debug.Log("server died");
        }
    }
}
