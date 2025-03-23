using System;
using UnityEngine;

public class CooldownCondition : Node
{
    private readonly float cooldownDuration;
    private float lastTriggerTime;

    public CooldownCondition(float cooldownDuration)
    {
        this.cooldownDuration = cooldownDuration;
        lastTriggerTime = -cooldownDuration; // Ermöglicht sofortiges Triggern
    }

    public override NodeState Run()
    {
        if (Time.time - lastTriggerTime >= cooldownDuration)
        {
            lastTriggerTime = Time.time; // Cooldown zurücksetzen
            return NodeState.Success;
        }

        return NodeState.Failure;
    }
}