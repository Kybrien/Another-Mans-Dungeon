using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class PlayerMovementController : NetworkBehaviour
{
    private RoundManager roundManager;

    [SerializeField] private GameObject PlayerModel;
    [SerializeField] private GameObject UICamera;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private RawImage healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Tooltip("Player Values")]

    [SerializeField] private float Speed = 10f;
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
            if (PlayerModel.activeSelf == false)
            {
                roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();

                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                PlayerModel.SetActive(true);
                UICamera.SetActive(true);
                SetSpawnPosition();
                Debug.Log(gameObject.name + " Position is: " + gameObject.transform.position);
            }

            if (isLocalPlayer)
            {
                Movement();
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

        transform.position += moveDirection * Speed * Time.deltaTime;

        if (moveDirection != Vector3.zero)
        {
            if (animator)
            {
                animator.SetBool("IsMoving", true);
            }
        }
        else
        {
            if (animator)
            {
                animator.SetBool("IsMoving", false);
            }

        }

        if (transform.position.y < -5000)
        {
            transform.position = new Vector3(transform.position.x, 50, transform.position.z);
        }
    }

    void UpdateHealthBar(float oldValue, float newValue)
    {
        healthBar.rectTransform.sizeDelta = new Vector2((newValue / maxHealth) * 250, 10);
        healthText.text = newValue.ToString() + " / " + maxHealth.ToString();

        if (newValue <= 0 && isDead == false) {
            isDead = true;
            roundManager.CmdPlayerDeath();
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
        }
    } 

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
