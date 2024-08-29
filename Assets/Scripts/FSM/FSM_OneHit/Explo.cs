using Pathfinding;
using UnityEngine;
using System.Collections;

public class ExploState : StateHit
{
    private AIPath aiPath;
    private Transform player;
    private float attackDistance = 4f;
    private SkinnedMeshRenderer monsterRenderer;

    public ExploState(MonsterControllerHit monsterController) : base(monsterController)
    {
        aiPath = monsterController.GetComponent<AIPath>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        monsterRenderer = monsterController.GetComponentInChildren<SkinnedMeshRenderer>(); // Trouver le SkinnedMeshRenderer dans les enfants
    }

    public override void EnterState()
    {
        aiPath.canMove = false;
        Debug.Log("Monster in Explo STATE");
        SetAnimationTrigger("isExplo");
        PerformExplo(); // Initialiser la premi�re attaque
    }

    public override void UpdateState()
    {
        RotateTowardsPlayer();
    }

    private void PerformExplo()
    {
        // D�marrer la coroutine pour attendre la fin de l'animation
        monsterController.StartCoroutine(WaitForAnimationAndExplode());
    }

    private IEnumerator WaitForAnimationAndExplode()
    {
        Animator animator = monsterController.GetComponent<Animator>();
        if (animator != null)
        {
            // Obtenir les informations sur l'�tat actuel de l'animation
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Attendre la dur�e de l'animation
            yield return new WaitForSeconds(stateInfo.length);

            // V�rifier si le monstre n'a pas �t� d�truit avant d'instancier l'explosion
            if (monsterController != null && monsterRenderer != null)
            {
                // D�sactiver le rendu du monstre
                monsterRenderer.enabled = false;

                // Instancier l'explosion � l'emplacement exact du monstre avec la m�me rotation
                GameObject explosion = GameObject.Instantiate(monsterController.explosionVFX, monsterController.transform.position, monsterController.transform.rotation);

                // Attendre que l'explosion se termine
                yield return new WaitForSeconds(1f);

                // D�truire le monstre et l'explosion
                GameObject.Destroy(monsterController.gameObject, 1f);
            }
        }
    }

    public override void ExitState()
    {
        // Arr�ter toutes les coroutines en cours lorsque l'�tat est quitt�
        monsterController.StopAllCoroutines();
    }

    private void RotateTowardsPlayer()
    {
        // Utiliser Lerp pour la rotation
        Vector3 direction = player.position - monsterController.transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        monsterController.transform.rotation = Quaternion.Lerp(monsterController.transform.rotation, toRotation, 10 * Time.deltaTime);
    }
}


