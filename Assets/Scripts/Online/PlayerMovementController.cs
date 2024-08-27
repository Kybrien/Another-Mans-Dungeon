using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using TMPro;
using Mirror.Examples.MultipleMatch;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private RoundManager roundManager;

    [SerializeField] private GameObject PlayerModel;
    [SerializeField] private GameObject PlayerGui;
    [SerializeField] private GameObject UICamera;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private RawImage healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private Animator _animator;

    [Tooltip("Player Values")]

    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 200f;

    [SyncVar(hook = nameof(UpdateHealthBar))]
    [SerializeField] private float health = 100;

    [SyncVar]
    [SerializeField] private bool isDead = false;

    [SyncVar]
    [SerializeField] private float maxHealth = 100;

    private void Start()
    {
        PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "OnlineGame")
        {
            if (roundManager == null)
            {
                GameObject roundManagerGO = GameObject.Find("RoundManager");
                if (roundManagerGO != null)
                {
                    roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
                }
            }
            if (PlayerModel.activeSelf == false)
            {
                GameObject SelectedModel = PlayerModel.transform.Find("Model" + (1 + NetworkClient.localPlayer.netId % 2).ToString()).gameObject;
                _animator = SelectedModel.GetComponent<Animator>();

                if (!isLocalPlayer)
                {
                    Debug.Log("other player");
                    SelectedModel.SetActive(true);
                }

                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                PlayerModel.SetActive(true);
                PlayerGui.SetActive(true);
                UICamera.SetActive(true);
                SetSpawnPosition();
                Debug.Log(gameObject.name + " Position is: " + gameObject.transform.position);
            }

            if (isLocalPlayer)
            {
                Movement();
            }

            if (!isLocalPlayer) {
                _animator.SetFloat("Forward", rb.velocity.z);
                _animator.SetFloat("Sided", rb.velocity.x);
            }
        }
    }

    public void SetSpawnPosition()
    {
        Physics.SyncTransforms();
        rb.velocity = Vector3.zero;
        transform.position = NetworkManager.singleton.GetStartPosition().position;
    }

    public void Movement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = (forward  * zDirection) + (right * xDirection);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce);
        }

        rb.MovePosition(rb.position + moveDirection * Time.deltaTime * speed);

        if (transform.position.y < -2000)
        {
            transform.position = new Vector3(transform.position.x, 50, transform.position.z);
            rb.velocity = Vector3.zero;
        }
    }

    public void TakeDamage(float value)
    {
        health = Mathf.Max(health - value, 0);
    }

    void UpdateHealthBar(float oldValue, float newValue)
    {
        Debug.Log("Health changed: " + newValue.ToString());
        healthBar.rectTransform.sizeDelta = new Vector2((newValue / maxHealth) * 400, healthBar.rectTransform.sizeDelta.y);
        healthText.text = newValue.ToString() + " / " + maxHealth.ToString();

        if (newValue <= 0 && isDead == false) {
            Debug.Log("Dead");
            isDead = true;
            roundManager.PlayerDied();
        }
    }

    public void UpdateStatus(int round, int timer)
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        timerText.text = "Time Left - " + string.Format("{0:00}:{1:00}", minutes, seconds);
        roundText.text = (round == 0 ? "Intermission" : "Round " + round.ToString());
    }

    [Server]
    public void SetHealth(float newHealth)
    {
        health = Mathf.Max(newHealth, maxHealth);
        if (health > 0)
        {
            isDead = false;
            Debug.Log("healed");
        }
    } 

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
