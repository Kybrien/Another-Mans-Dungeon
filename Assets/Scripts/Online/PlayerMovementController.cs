using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 10f;
    public float jumpForce = 200f;
    public GameObject PlayerModel;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

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
                rb.useGravity = true;
                PlayerModel.SetActive(true);
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

        if (transform.position.y < -50)
        {
            transform.position = new Vector3(transform.position.x, 50, transform.position.z);
        }
    }
}
