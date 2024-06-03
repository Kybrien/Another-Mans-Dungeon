using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolRangedState : StateRanged 
{
    private Transform player;
    public PatrolRangedState(MonsterControllerRanged monsterController) : base(monsterController)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void EnterState()
    {
        Debug.Log("Monster in Patrol STATE");
        // Logique d'initialisation de l'�tat Idle
    }

    public override void Update()
    {

    }

    public override void ExitState()
    {
        // Logique de nettoyage de l'�tat de Chase
    }
}
