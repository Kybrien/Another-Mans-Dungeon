using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PortalManager : NetworkBehaviour
{
    private RoundManager roundManager;

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && gameObject.name == "PortalEnd")
        {
            NetworkIdentity plrIdentity = other.GetComponent<NetworkIdentity>();

            if (plrIdentity) {
                roundManager.CmdInvadeWorld(other.gameObject);
            }
        }
    }
}
