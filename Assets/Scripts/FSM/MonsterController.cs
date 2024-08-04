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
        Vector3 position = canvas.transform.position;
        Vector3 target = NetworkClient.localPlayer.gameObject.transform.position;
        Vector3 inverseHeight = new Vector3(0, (position.y - target.y) * 2, 0);
        canvas.transform.LookAt(2 * (position + inverseHeight) - target);
    }

    public float GetHealth()
    {
        return health;
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
        Debug.Log("SERVER HEALTH: " + health.ToString());
    }

    void UpdateHealthBar(float oldValue, float newValue)
    { 
        healthBar.rectTransform.sizeDelta = new Vector2((newValue / maxHealth) * 5, 1);
        healthText.text = newValue.ToString() + " / " + maxHealth.ToString();
    }

    void LocalUpdateHealthBar()
    {
        healthBar.rectTransform.sizeDelta = new Vector2((health / maxHealth) * 5, 1);
        healthText.text = health.ToString() + " / " + maxHealth.ToString();
    }
}
