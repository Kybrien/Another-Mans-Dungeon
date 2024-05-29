using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ChaseState : State
{
    private AIPath aiPath;
    private Transform player;
    public float initialChaseRadius = 13f;
    public float increasedChaseRadius = 16f;
    private float attackDistance = 4f;
    public float currentChaseRadius;
    private bool playerDetected;

    public ChaseState(MonsterController monsterController) : base(monsterController)
    {
        aiPath = monsterController.GetComponent<AIPath>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Assurez-vous que le joueur a ce tag
        currentChaseRadius = initialChaseRadius;
        playerDetected = false;
    }

    public override void EnterState()
    {
        Debug.Log("Monster in Chase STATE");
        // Logique d'initialisation de l'état de Chase
    }

    public override void Update()
    {
        float distanceToPlayer = Vector3.Distance(monsterController.transform.position, player.position);

        if (distanceToPlayer < currentChaseRadius)
        {
            if (!playerDetected)
            {
                playerDetected = true;
                currentChaseRadius = increasedChaseRadius;
            }
            aiPath.destination = player.position;
            SetAnimationTrigger("isChasing");
            aiPath.canMove = true;

            if (distanceToPlayer <= attackDistance)
            {
                monsterController.TransitionToState(monsterController.attackState);
            }
        }
        else
        {
            if (playerDetected && distanceToPlayer >= currentChaseRadius)
            {
                playerDetected = false;
                currentChaseRadius = initialChaseRadius;
            }
            aiPath.canMove = false;
            monsterController.TransitionToState(monsterController.idleState);
        }
    }

    public override void ExitState()
    {
        // Logique de nettoyage de l'état de Chase
    }
}
