public class Sequence : Node
{
  public override NodeState Run()
  {
    if (currentChildIndex < children.Count) {
      switch (children[currentChildIndex].Run()) {
        case NodeState.Running:
          return NodeState.Running;
        case NodeState.Failure:
          currentChildIndex = 0;
          return NodeState.Failure;
        default:
          currentChildIndex++;
          return currentChildIndex == children.Count ? NodeState.Success : NodeState.Running;
      }
    }

    Reset();
    return NodeState.Success;
  }

  public override void Reset() {
    currentChildIndex = 0;
  }
}
