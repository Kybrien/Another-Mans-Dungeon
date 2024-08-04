using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : NetworkBehaviour
{
    [SyncVar]
    private List<GameObject> debounce;
    // Start is called before the first frame update
    void Start()
    {
        debounce = new List<GameObject>();
        for (int i = 0; i < debounce.Count; i++)
        {
            Debug.Log(debounce[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLocalPlayer)
        {
            Debug.Log("degats local!");
        }

        if (isServer)
        {
            Debug.Log("degats serveur");
        }

        if (other.tag == "Enemy")
        {
            if (debounce.Find((x) => x == other.gameObject)) {
                Debug.Log(other.gameObject.name + " already in list");
                return;
            }

            debounce.Add(other.gameObject);

            MonsterController monsterHealth = other.GetComponent<MonsterController>();
            monsterHealth.TakeDamage(10);

            //monsterHealth.LocalUpdateHealthBar();
            Debug.Log("HEALTH: " + monsterHealth.GetHealth());
        }
    }
}
