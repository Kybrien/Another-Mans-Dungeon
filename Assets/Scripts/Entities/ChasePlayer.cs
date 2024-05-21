using UnityEngine;
using Pathfinding;

public class ChasePlayer : MonoBehaviour
{
    public Transform player;
    private AIPath aiPath;
    private Rigidbody rb;
    public float initialChaseRadius = 13f;
    public float increasedChaseRadius = 16f;
    private float currentChaseRadius;
    private bool playerDetected;
    

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        rb = GetComponent<Rigidbody>();
        currentChaseRadius = initialChaseRadius;
        playerDetected = false;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < currentChaseRadius)
        {
            if (!playerDetected)
            {
                playerDetected = true;
                currentChaseRadius = increasedChaseRadius;
                Debug.Log(currentChaseRadius);
            }
            aiPath.destination = player.position;
            aiPath.canMove = true;
        }
        else
        {
            if (playerDetected && distanceToPlayer >= currentChaseRadius)
            {
                playerDetected = false;
                currentChaseRadius = initialChaseRadius;
            }
            aiPath.canMove = false;
        }

        CheckGrounded();
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            // Si le monstre n'est pas en contact avec le sol, applique une force vers le bas
            rb.AddForce(Vector3.down * 20f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, initialChaseRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, increasedChaseRadius);
    }
}
