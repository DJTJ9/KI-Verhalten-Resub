using UnityEngine;

public class TargetInRangeCheck : Node
{
    private Transform dog;
    private Transform target;
    private float range;

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
