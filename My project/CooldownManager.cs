using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class : MonoBehaviour
{
    public float cooldownBeforeSend = 2.0f;
    private float cooldownTimer;

    public bool IsCooldownActive => cooldownTimer > 0;

    public void StartCooldown()
    {
        cooldownTimer = cooldownBeforeSend;
    }

    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
}
