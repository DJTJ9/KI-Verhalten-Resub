using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    private List<Node> children = new();

    public Selector(List<Node> nodes)
    {
        children = nodes;
    }

    public override NodeState Run()
    {
        foreach (var child in children)
        {
            switch (child.Run())
            {
                case NodeState.Success:
                    return NodeState.Success;
                case NodeState.Running:
                    return NodeState.Running;
            }
        }

        return NodeState.Failure;
    }
}