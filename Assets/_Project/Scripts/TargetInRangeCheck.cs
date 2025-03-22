using UnityEngine;

public class TargetInRangeCheck : Node
{
    private readonly Transform dog;
    private readonly Transform target;
    private readonly float range;

    public TargetInRangeCheck(Transform dog, Transform target, float range)
    {
        this.dog = dog;
        this.target = target;
        this.range = range;
    }

    public override NodeState Run()
    {
        float distance = Vector3.Distance(dog.position, target.position);
        return distance < range ? NodeState.Success : NodeState.Failure;
    }
}
