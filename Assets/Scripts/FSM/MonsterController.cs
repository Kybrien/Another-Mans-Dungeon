using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : NetworkBehaviour
{
    [Header("-- Monster Stats --")]

    [SyncVar]
    [SerializeField] private float maxHealth = 100;
    [SyncVar(hook = nameof(UpdateHealthBar))]
    [SerializeField] private float health = 100;
    [SyncVar]
    private bool isDead = false;

    [SerializeField] private Canvas canvas;
    [SerializeField] private RawImage healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public float GetHealth()
    {
        return health;
    }

    [Server]
    public void TakeDamage(int damage)
    {
        if (!isServer) return;

        health = Mathf.Max(0, health - damage);

        if (isDead == false && health == 0)
        {
            isDead = true;
            OnDeath();
            NetworkServer.UnSpawn(gameObject);
        }
    }

    void UpdateHealthBar(float oldValue, float newValue)
    { 
        healthBar.rectTransform.sizeDelta = new Vector2((newValue / maxHealth) * 5, 1);
        healthText.text = newValue.ToString() + " / " + maxHealth.ToString();

        Debug.Log("health updated");
    }

    void LocalUpdateHealthBar()
    {
        healthBar.rectTransform.sizeDelta = new Vector2((health / maxHealth) * 5, 1);
        healthText.text = health.ToString() + " / " + maxHealth.ToString();
    }

    void OnDeath()
    {

    }
}
