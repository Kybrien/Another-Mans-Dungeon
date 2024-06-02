using UnityEngine;

public class RangedState: State
{
    private Transform player;

    public RangedState(MonsterController monsterController) : base(monsterController)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void EnterState()
    {

    }

    public override void Update()
    {

    }

    public override void ExitState()
    {
        // Logique de nettoyage de l'état de Chase
    }
}
