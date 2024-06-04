using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRangedState : StateRanged 
{
    private AIPath aiPath;
    private Vector3 basePosition;
    private Vector3 patrolTarget;
    private float patrolRadius = 15f;
    private float distanceThreshold = 0.5f; // Distance minimale pour consid�rer que le monstre est arriv� � destination

    public PatrolRangedState(MonsterControllerRanged monsterController) : base(monsterController)
    {
        aiPath = monsterController.GetComponent<AIPath>();
        basePosition = monsterController.transform.position;
    }

    public override void EnterState()
    {
        Debug.Log("Monster in Patrol STATE");
        // Logique d'initialisation de l'�tat Idle
        SetNewPatrolTarget();
    }

    public override void Update()
    {
        //Le monstre patrouille aleatoirement sur une zone
        SetAnimationTrigger("isPatrolling");

        //Passage a l'etat d'attaque si le joueur est detecte
        if (Vector3.Distance(monsterController.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < monsterController.initialRadius) // distance de d�tection
        {
            monsterController.TransitionToState(monsterController.rangedAttackState);
        }

        // V�rifier si le monstre est arriv� � destination
        if (Vector3.Distance(monsterController.transform.position, patrolTarget) <= distanceThreshold)
        {
            // D�finir un nouveau point de patrouille ou retourner � l'�tat idle
            monsterController.TransitionToState(monsterController.idleState);
        }

    }

    public override void ExitState()
    {
        // Logique de nettoyage de l'�tat de Chase
        aiPath.canMove = false;
    }

    private void SetNewPatrolTarget()
    {
        // G�n�rer une nouvelle position de patrouille al�atoire dans un rayon de 7 unit�s
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += basePosition;
        randomDirection.y = basePosition.y; // Assurez-vous que le y reste constant si vous �tes sur un terrain plat

        patrolTarget = randomDirection;

        aiPath.destination = patrolTarget;
        aiPath.canMove = true;
    }
}
