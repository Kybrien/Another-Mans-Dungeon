using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : NetworkBehaviour
{
    private SyncList<GameObject> debounce;
    // Start is called before the first frame update
    void Start()
    {
        debounce = new SyncList<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
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
