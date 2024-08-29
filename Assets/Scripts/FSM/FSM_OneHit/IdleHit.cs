using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleHitState : StateHit
{
    public IdleHitState(MonsterControllerHit monsterControllerHit) : base(monsterControllerHit) { }

    public override void EnterState()
    {
        Debug.Log("Monster in Idle STATE");
        // Logique d'initialisation de l'�tat Idle
    }
    public override void UpdateState()
    {
        SetAnimationTrigger("isIdle");
        // Logique pour l'�tat Idle
        if (Vector3.Distance(monsterController.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < monsterController.initialChaseRadius) // distance de d�tection par exemple
        {
            monsterController.TransitionToState(monsterController.chaseState);
            //monsterController.TransitionToState(monsterController.rushState);
        }
    }

    public override void ExitState()
    {
        // Logique de nettoyage de l'�tat Idle
    }
}
