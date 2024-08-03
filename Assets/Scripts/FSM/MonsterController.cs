using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : NetworkBehaviour
{
    [Header("-- Monster Stats --")]

    [SyncVar(hook = nameof(UpdateHealthBar))]
    [SerializeField] private float maxHealth = 100;
    [SyncVar(hook = nameof(UpdateHealthBar))]
    [SerializeField] private float health = 100;

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
        canvas.transform.LookAt(NetworkClient.localPlayer.gameObject.transform.position);

    }

    public float GetHealth()
    {
        return health;
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
    }

    void UpdateHealthBar(float oldValue, float newValue)
    {
        healthBar.rectTransform.sizeDelta = new Vector2((health / maxHealth) * 5, 1);
        healthText.text = health.ToString() + " / " + maxHealth.ToString();
    }
}
