using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRangedState : StateRanged
{
    private Transform player;
    public IdleRangedState(MonsterControllerRanged monsterController) : base(monsterController)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void EnterState()
    {
        Debug.Log("Monster in Idle STATE");
        // Logique d'initialisation de l'état Idle
    }

    public override void Update()
    {

    }

    public override void ExitState()
    {
        // Logique de nettoyage de l'état de Chase
    }
}
