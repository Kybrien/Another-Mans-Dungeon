using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MonsterController : MonoBehaviour
{
    public Transform player;
    public AIPath aiPath;
    public Animator animator;
    public Rigidbody rb;

    private State currentState;
    public State idleState;
    public State chaseState;
    public State attackState;
    public State rushState;
    public State rangedState;
    public State takingDamageState;
    public State deadState;

    //HEADER for inspector
    [Header("-- Monster Stats --")]
    [Tooltip("Distance initiale de detection...")]
    [SerializeField][Range(8f, 20f)] public float initialChaseRadius = 13f;
    public float increasedChaseRadius => initialChaseRadius + 3f;

    [Tooltip("Distance d'attaque...")]
    [SerializeField][Range(2f, 12f)] public float attackDistance = 4f;

    [Tooltip("Le monstre peut-il rush ?")]
    [SerializeField] public bool CanRush = false;
    [Tooltip("Vitesse de rush...")]
    [SerializeField] public float rushSpeedMultiplier = 1.3f;

    [Tooltip("Description de la stratégie d'attaque du monstre.")]
    [TextArea(3, 5)]
    [SerializeField] private string attackStrategyDescription = "Le monstre attaque de manière agressive lorsque le joueur est à portée.";


    [Header("-- Ranged Attack --")]
    [Tooltip("Le monstre peut-il attaquer à distance ?")]
    [SerializeField] public bool isRanged = false;

    [Tooltip("Prefab de l'attaque simple à distance.")]
    [SerializeField] public GameObject rangedAttackPrefab1;

    [Tooltip("Prefab de l'attaque lourde à distance.")]
    [SerializeField] public GameObject rangedAttackPrefab2;


    void Start()
    {
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        rushState = new RushState(this);
        rangedState = new RangedState(this);
        //takingDamageState = new TakingDamageState(this);
        //deadState = new DeadState(this);

        currentState = idleState; // Début en état Idle
        currentState.EnterState(); // Entrer dans l'état initial
    }

    void Update()
    {
        currentState.Update();
    }

    public void TransitionToState(State nextState)
    {
        currentState.ExitState();
        currentState = nextState;
        currentState.EnterState();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Déléguer la gestion de collision à l'état actuel
        if (currentState is RushState rushState)
        {
            rushState.OnCollisionEnter(collision);
        }
    }
}