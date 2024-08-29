using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UIElements;

public class PortalManager : NetworkBehaviour
{
    private RoundManager roundManager;

    [SyncVar]
    private bool canEnter = true;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
        audioSource = GetComponent<AudioSource>(); 
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canEnter) return;

        if (other.tag == "Player" && gameObject.name == "PortalEnd")
        {
            NetworkIdentity plrIdentity = other.GetComponent<NetworkIdentity>();

            if (plrIdentity)
            {
                if (!hasPlayed && audioSource != null)
                {
                    audioSource.Play();
                    hasPlayed = true; 
                }

                CmdDisablePortal();
                roundManager.CmdInvadeWorld(gameObject, other.gameObject);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdDisablePortal()
    {
        canEnter = false;
    }
}
