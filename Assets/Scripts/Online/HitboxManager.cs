using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
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
        Debug.Log("touched");
        if (other.tag == "Enemy")
        {
            if (debounce.Find((x) => x == other.gameObject)) {
                return;
            }

            MonsterController monsterHealth = other.GetComponent<MonsterController>();

            debounce.Add(other.gameObject);
            monsterHealth.TakeDamage(10);

            Debug.Log("HEALTH: " + monsterHealth.GetHealth());
        } else
        {
            Debug.Log("already in list");
        }
    }
}
