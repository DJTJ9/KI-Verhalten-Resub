using UnityEngine;
using System.Collections.Generic;

public abstract class Node
{
    public enum NodeState {Running, Success, Failure}
    
    protected NodeState state;
    protected int currentChildIndex;
    
    protected readonly List<Node> children = new();
    
    public void Add(Node node) => children.Add(node);
    
    public virtual NodeState Run() => children[currentChildIndex].Run();

    public virtual void Reset() {
        currentChildIndex = 0;
        foreach (var child in children) child.Reset();
    }
}
