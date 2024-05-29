using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackState : State
{
    private float attackDistance = 4f;
    private float attackInterval = 0.5f; // Intervalle entre les attaques
    private float nextAttackTime; // Temps de la prochaine attaque

    public AttackState(MonsterController monsterController) : base(monsterController) { }

    public override void EnterState()
    {
        Debug.Log("Monster in Attack STATE");
        nextAttackTime = Time.time + attackInterval; // Initialiser le temps de la prochaine attaque
    }

    public override void Update()
    {
        float distanceToPlayer = Vector3.Distance(monsterController.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

        if (distanceToPlayer > attackDistance) // Si le joueur est hors de port�e d'attaque
        {
            monsterController.TransitionToState(monsterController.chaseState);
        }
        else if (Time.time >= nextAttackTime) // Si le temps de la prochaine attaque est atteint
        {
            PerformAttack();
            nextAttackTime = Time.time + attackInterval; // Mettre � jour le temps de la prochaine attaque
        }
    }

    private void PerformAttack()
    {
        // G�n�rer un nombre al�atoire entre 1 et 3
        int random = Random.Range(1, 4);
        Debug.Log("Random Attack Chosen: " + random);

        if (random == 1)
        {
            SetAnimationTrigger("isAttacking");
        }
        else if (random == 2)
        {
            SetAnimationTrigger("isAttacking2");
        }
        else if (random == 3)
        {
            SetAnimationTrigger("isAttacking3");
        }

        Debug.Log("Performing attack: " + random);
    }

    public override void ExitState()
    {
        // Logique de nettoyage de l'�tat d'attaque
    }
}
