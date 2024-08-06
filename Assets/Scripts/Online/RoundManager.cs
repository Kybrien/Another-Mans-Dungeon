using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class RoundManager : NetworkBehaviour
{
    [SerializeField] private GameObject test;

    [Header("GAME SETTINGS")]

    [SerializeField] private int rounds = 5;

    [SerializeField] private int maxRoundTimer = 5;

    [Header("GAME PROGRESSION (DO NOT TOUCH)")]

    [SyncVar]
    private string currentMap = "0";

    [SyncVar]
    private int currentRound = 0;

    [SyncVar]
    private int secondsLeft = 30;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            Debug.Log("Start!");
            RpcSwitchMap("Test");
            StartCoroutine(HandleRounds());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator HandleRounds()
    {
        while (currentRound < rounds)
        {
            if (secondsLeft > 0)
            {
                secondsLeft -= 1;
            } else
            {
                currentRound += 1;
            }

            yield return new WaitForSeconds(1);
        }
    }

    [ClientRpc]
    void RpcSwitchMap(string mapName)
    {
        if (isLocalPlayer)
        {
            Debug.Log(mapName + " map spawned in local!");
        }

        if (isServer)
        {
            Debug.Log(mapName + " map spawned on server!");
        }

        GameObject NewHitbox = Instantiate(test);
        NewHitbox.transform.position = new Vector3(0, 5, 10000);
    }
}
