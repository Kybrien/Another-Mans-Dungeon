using Org.BouncyCastle.Cms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using Mirror;

public class CombatController : NetworkBehaviour
{
    bool isClicking = false;
    [SerializeField] private GameObject HitboxPrefab;
    private Transform ModelRoot;

    // TEMP VALUES
    float cooldown = 0.5f;
    float currCooldown = 0;
    bool isRange = true;

    // Start is called before the first frame update
    void Start()
    {
        ModelRoot = gameObject.transform.Find("ModelRoot");
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

    private GameObject CreateHitbox(Vector3 size)
    {
        GameObject NewHitbox = Instantiate(HitboxPrefab);
        NewHitbox.transform.position = ModelRoot.position + ModelRoot.forward * 3;
        NewHitbox.transform.rotation = ModelRoot.rotation;
        NewHitbox.transform.localScale = size;
        Destroy(NewHitbox, 0.1f);

        return NewHitbox;
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
        float state = context.action.ReadValue<float>();
        if (isClicking && state == 0)
        {
            isClicking = false;
        }
        else if (!isClicking && state > 0)
        {
            isClicking = true;
        } else
        {
            return;
        }

        HandleCombat(isClicking);
    }

    private void HandleCombat(bool pressing)
    {
        if (pressing && currCooldown <= 0)
        {
            if (isRange)
            {
                RaycastHit result = SendRaycast();
                Debug.Log(result.transform.gameObject.name);
            }
            else
            {
                Debug.Log("Can fight");
                CreateHitbox(new Vector3(4, 4, 4));
                currCooldown = cooldown;
            }
        }
    }
}
