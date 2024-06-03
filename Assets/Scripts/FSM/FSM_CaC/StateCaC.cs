using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateCaC
{
    protected MonsterControllerCaC monsterController;

    public StateCaC(MonsterControllerCaC monsterController)
    {
        this.monsterController = monsterController;
    }

    // M�thodes � appeler lors de l'entr�e et de la sortie d'un �tat
    public virtual void EnterState() { }
    public virtual void ExitState() { }

    // M�thode abstraite pour la mise � jour de l'�tat
    public abstract void Update();

    // M�thode pour d�finir un trigger d'animation
    protected void SetAnimationTrigger(string triggerName)
    {
        monsterController.animator.SetTrigger(triggerName);
    }
}