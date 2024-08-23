using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : NetworkBehaviour
{
    public NetworkIdentity plrIdentity;
    private SyncList<GameObject> debounce = new SyncList<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" || (other.tag == "Player" && other.GetComponent<NetworkIdentity>().netId != plrIdentity.netId))
        {
            if (debounce.Find((x) => x == other.gameObject)) {
                Debug.Log(other.gameObject.name + " already in list");
                return;
            }

            debounce.Add(other.gameObject);

            if (other.tag == "Enemy")
            {
                MonsterController monsterHealth = other.GetComponent<MonsterController>();
                monsterHealth.TakeDamage(10);
            } else if (other.tag == "Player")
            {
                PlayerMovementController playerHealth = other.GetComponent<PlayerMovementController>();
                playerHealth.TakeDamage(10);
            }

            //monsterHealth.LocalUpdateHealthBar();
        }
    }
}
