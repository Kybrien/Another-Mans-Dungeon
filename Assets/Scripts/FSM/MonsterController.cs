using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MonsterController : MonoBehaviour
{
    public Transform player;
    public AIPath aiPath;
    public Animator animator;

    private State currentState;

    public State idleState;
    public State chaseState;
    public State attackState;
    public State rushState;
    public State ranged1State;
    public State ranged2State;
    public State takingDamageState;
    public State deadState;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();

        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        //rushState = new RushState(this);
        //ranged1State = new Ranged1State(this);
        //ranged2State = new Ranged2State(this);
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
}