using System;

public class ActionNode : Node
{
    private readonly Action action;

    public ActionNode(Action action) {
        this.action = action;
    }

    public override NodeState Run() {
        action();
        return NodeState.Success;
    }
}