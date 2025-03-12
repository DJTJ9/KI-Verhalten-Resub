using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
  private List<Node> children = new();

  public Sequence(List<Node> nodes)
  {
    children = nodes;
  }

  public override NodeState Run()
  {
    foreach (var child in children)
    {
      if (child.Run() == NodeState.Failure) return NodeState.Failure;
    }
    return NodeState.Success;
  }
}
