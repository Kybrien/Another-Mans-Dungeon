using UnityEngine;

public class RangedAttackState: StateRanged
{
    private Transform player;
    public RangedAttackState(MonsterControllerRanged monsterController) : base(monsterController)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void EnterState()
    {
        Debug.Log("Monster in Ranged Attack STATE");
    }

    public override void Update()
    {

    }

    public override void ExitState()
    {
        // Logique de nettoyage de l'état de Chase
    }
}
