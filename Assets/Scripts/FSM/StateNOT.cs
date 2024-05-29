using Pathfinding.RVO.Sampled;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

public abstract class StateNOT
{
    public enum STATE
    {
        IDLE, CHASE, RUSH, ATTACK, TAKING_DAMAGE, DEAD
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    protected Transform player;
    protected StateNOT nextState;
    protected AIPath aiPath;

    protected float visDist = 9.0f;
    protected float visAngle = 360.0f;

    public StateNOT(GameObject _npc, AIPath _aiPath, Animator _anim, Transform _player)
    {
        npc = _npc;
        aiPath = _aiPath;
        anim = _anim;
        stage = EVENT.ENTER;
        player = _player;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.UPDATE; }

    public StateNOT Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);
        if (direction.magnitude < visDist && angle < visAngle)
        {
            return true;
        }
        return false;
    }

    public bool CanAttackPlayer()
    {
        float distance = Vector3.Distance(player.position, npc.transform.position);
        Debug.Log($"Distance to player: {distance}, Attack Distance: {aiPath.endReachedDistance}");
        if (distance < aiPath.endReachedDistance)
        {
            Debug.Log("Player in attack range.");
            return true;
        }
        Debug.Log("Player not in attack range.");
        return false;
    }
}

public class Idlea : StateNOT
{
    public Idlea(GameObject _npc, AIPath _aiPath, Animator _anim, Transform _player)
        : base(_npc, _aiPath, _anim, _player)
    {
        //passage a l'idle
        name = STATE.IDLE;
    }

    public override void Enter()
    {
        anim.SetTrigger("isIdle");
        Debug.Log("Idle STATE");
        base.Enter();
    }

    public override void Update()
    {
        if (CanSeePlayer())
        {
            nextState = new Chasea(npc, aiPath, anim, player);
            Debug.Log("Transitioning to Chase state.");
            stage = EVENT.EXIT;
        }
        base.Update();
    }

    public override void Exit()
    {
        anim.ResetTrigger("isIdle");
        base.Exit();
    }
}



public class Chasea : StateNOT
{
    private Rigidbody rb;
    private float initialChaseRadius = 15f;
    private float increasedChaseRadius = 20f;
    private float currentChaseRadius;
    private bool playerDetected;

    public Chasea(GameObject _npc, AIPath _aiPath, Animator _anim, Transform _player)
        : base(_npc, _aiPath, _anim, _player)
    {
        name = STATE.CHASE;
        rb = _npc.GetComponent<Rigidbody>();
        currentChaseRadius = initialChaseRadius;
        playerDetected = true;
    }

    public override void Enter()
    {
        stage = EVENT.UPDATE;
        Debug.Log("Chase STATE");
        anim.SetTrigger("isChasing");
        /*aiPath.canSearch = true;
        aiPath.SearchPath();*/
        aiPath.destination = player.position;
        aiPath.canMove = true;
        base.Enter();
    }

    public override void Update()
    {
        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);

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
                aiPath.canMove = false;
            }
        }

        if (CanAttackPlayer())
        {
            Debug.Log("Transitioning to Attack state.");
            nextState = new Attacka(npc, aiPath, anim, player);
            stage = EVENT.EXIT;
        }
        else if (!CanSeePlayer())
        {
            Debug.Log("Lost sight of player, transitioning to Idle state.");
            nextState = new Idlea(npc, aiPath, anim, player);
            stage = EVENT.EXIT;
        }

        base.Update();
    }

    public override void Exit()
    {
        anim.ResetTrigger("isChasing");
        aiPath.canMove = false;
        base.Exit();
    }
}



public class Attacka : StateNOT
{
    float rotationSpeed = 2.0f;

    public Attacka(GameObject _npc, AIPath _aiPath, Animator _anim, Transform _player)
        : base(_npc, _aiPath, _anim, _player)
    {
        name = STATE.ATTACK;
    }

    public override void Enter()
    {
        Debug.Log("Attack STATE");
        anim.SetTrigger("isAttacking");
        aiPath.canMove = false;
        stage = EVENT.UPDATE;
        base.Enter();
    }

    public override void Update()
    {
        Vector3 direction = player.position - npc.transform.position;
        direction.y = 0;

        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  Time.deltaTime * rotationSpeed);

        if (!CanAttackPlayer())
        {
            Debug.Log("Player out of attack range, transitioning to Chase state.");
            nextState = new Chasea(npc, aiPath, anim, player);
            stage = EVENT.EXIT;
        }
        base.Update();
    }

    public override void Exit()
    {
        anim.ResetTrigger("isAttacking");
        aiPath.canMove = true;
        base.Exit();
    }
}


/*public class Rush : State
{
    public Rush(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        heavyState = STATE_Heavy.RUSH;
    }

    public override void Enter()
    {
        anim.SetTrigger("isRushing");
        agent.speed = 10.0f; // Increase speed for rush
        agent.SetDestination(player.position);
        base.Enter();
    }

    public override void Update()
    {
        if (Vector3.Distance(npc.transform.position, player.position) < attackDist)
        {
            nextState = new Attack(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
        else
        {
            agent.SetDestination(player.position);
        }
        base.Update();
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRushing");
        agent.speed = 5.0f; // Reset speed
        base.Exit();
    }
}*/

/*public class TakingDamage : State
{
    public TakingDamage(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        lightState = STATE_Light.TAKING_DAMAGE;
    }

    public override void Enter()
    {
        anim.SetTrigger("isTakingDamage");
        base.Enter();
    }

    public override void Update()
    {
        // Take damage logic here
        // ...

        if (npc.GetComponent<Health>().health <= 0)
        {
            nextState = new Dead(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
        else
        {
            nextState = new Idle(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
        base.Update();
    }

    public override void Exit()
    {
        anim.ResetTrigger("isTakingDamage");
        base.Exit();
    }
}*/

/*public class Dead : State
{
    public Dead(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        lightState = STATE_Light.DEAD;
    }

    public override void Enter()
    {
        anim.SetTrigger("isDead");
        agent.isStopped = true;
        base.Enter();
    }

    public override void Update()
    {
        // Dead logic here
        // ...
        base.Update();
    }

    public override void Exit()
    {
        // No exit from dead state
        base.Exit();
    }
}*/