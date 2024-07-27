using Org.BouncyCastle.Cms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using Mirror;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using Steamworks;

public class CombatController : NetworkBehaviour
{
    bool isClicking = false;
    [SerializeField] private GameObject HitboxPrefab;
    [SerializeField] private Transform ModelRoot;
    private Animator PlayerAnimator;
    private int comboCount = 0;
    private Coroutine comboCoroutine;

    // TEMP VALUES
    string weaponType = "Axe";
    float cooldown = 0.3f;
    float currCooldown = 0;
    bool isRange = false;

    // Start is called before the first frame update
    void Start()
    {
        //PlayerAnimator = ModelRoot.transform.Find("Model").GetComponent<Animator>();
        //SetAnimation(weaponType + "Idle");
    }

    // Update is called once per frame
    void Update()
    {
        if (currCooldown > 0)
        {
            currCooldown = Math.Max(0, currCooldown - Time.deltaTime);
        }
    }
    private void FixedUpdate()
    {
        SendDebugRaycast();
    }

    private void SetAnimation(string animationName)
    {
        PlayerAnimator.SetTrigger(animationName);
    }

    [Command]
    private void CmdCreateHitbox(Vector3 size)
    {
        GameObject NewHitbox = Instantiate(HitboxPrefab);
        NewHitbox.transform.position = ModelRoot.position + ModelRoot.forward * 3;
        NewHitbox.transform.rotation = ModelRoot.rotation;
        NewHitbox.transform.localScale = size;
        NetworkServer.Spawn(NewHitbox);
        Destroy(NewHitbox, 0.1f);
    }

    [Command]
    private void CmdDealMonsterDamage(RaycastHit result)
    {
        if (result.transform.tag == "Enemy")
        {
            result.transform.gameObject.GetComponent<MonsterController>().TakeDamage(10);
        }
    }

    private void SendDebugRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(ModelRoot.transform.position, ModelRoot.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(ModelRoot.transform.position, ModelRoot.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(ModelRoot.transform.position, ModelRoot.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Did not Hit");
        }
    }

    private RaycastHit SendRaycast()
    {

        RaycastHit hit;
        Physics.Raycast(ModelRoot.transform.position, ModelRoot.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity);

        return hit;
    }

    public void HandleMouseClick(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) { return; }

        float state = context.action.ReadValue<float>();
        if (isClicking && state == 0)
        {
            isClicking = false;
        }
        else if (!isClicking && state > 0)
        {
            isClicking = true;
        }
        else
        {
            return;
        }

        HandleCombat(isClicking);
    }

    private void ResetCombo()
    {
        comboCount = 0;
        //SetAnimation(weaponType + "Idle");
    }

    private IEnumerator WaitAndResetCombo()
    {
        // Attendre 1 seconde
        yield return new WaitForSeconds(1f);
        ResetCombo();
    }

    private IEnumerator ComboTick()
    {
        //SetAnimation(weaponType + "Attack" + comboCount.ToString());
        yield return new WaitForSeconds(1f);
        ResetCombo();
    }

    private void HandleCombo()
    {
        if (comboCount >= 4)
        {
            return;
        }

        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
        }

        comboCount++;

        if (comboCount >= 4)
        {
            comboCoroutine = StartCoroutine(WaitAndResetCombo());
        }
        else
        {
            comboCoroutine = StartCoroutine(ComboTick());
        }

        if (isRange)
        {
            RaycastHit result = SendRaycast();
            Debug.Log(result.transform.gameObject.name);
            CmdDealMonsterDamage(result);
        }
        else
        {
            CmdCreateHitbox(new Vector3(4, 4, 4));
            currCooldown = cooldown;
        }

        //CreateHitbox(new Vector3(1, 1, 1));
    }
    private void HandleCombat(bool pressing)
    {
        if (pressing && currCooldown <= 0)
        {
            HandleCombo();
        }
    }
}
