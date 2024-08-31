using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : StateCaC
{
    private float attackDistance = 4f;
    private float attackInterval = 0.5f; // Intervalle entre les attaques

    public AttackState(MonsterControllerCaC monsterController) : base(monsterController) { }

    public override void EnterState()
    {
        //Debug.Log("Monster in Attack STATE");
        PerformAttack(); // Initialiser la premi�re attaque
    }

    public override void Update()
    {
        float distanceToPlayer = Vector3.Distance(monsterController.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

        if (distanceToPlayer > attackDistance) // Si le joueur est hors de port�e d'attaque
        {
            monsterController.TransitionToState(monsterController.chaseState);
        }
    }

    private void PerformAttack()
    {
        monsterController.StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        while (Vector3.Distance(monsterController.transform.position, Player.transform.position) <= attackDistance)
        {
            // G�n�rer un nombre al�atoire entre 1 et 3
            int random = Random.Range(1, 4);
            //Debug.Log("Random Attack Chosen: " + random);

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

            //Debug.Log("Performing attack: " + random);

            Player.GetComponent<PlayerMovementController>().TakeDamage(Player, 5);

            // Attendre l'intervalle avant de permettre une nouvelle attaque
            yield return new WaitForSeconds(attackInterval);
        }

        // Si le joueur sort de la port�e d'attaque, retourner � l'�tat de poursuite
        monsterController.TransitionToState(monsterController.chaseState);
    }

    public override void ExitState()
    {
        // Arr�ter toutes les coroutines en cours lorsque l'�tat est quitt�
        monsterController.StopAllCoroutines();
    }
}

