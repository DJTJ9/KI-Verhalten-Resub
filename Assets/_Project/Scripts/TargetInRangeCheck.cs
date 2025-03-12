using UnityEngine;

public class TargetInRangeCheck : Node
{
    private Transform npc;
    private Transform target;
    private float range;

    public TargetInRangeCheck(Transform npc, Transform target, float range)
    {
        this.npc = npc;
        this.target = target;
        this.range = range;
    }

    public override NodeState Run()
    {
        float distance = Vector3.Distance(npc.position, target.position);
        return distance < range ? NodeState.Success : NodeState.Failure;
    }
}
