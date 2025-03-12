using UnityEngine;
using System.Collections.Generic;

public abstract class Node
{
    public enum NodeState {Running, Success, Failure}
    protected NodeState state;
    
    public abstract NodeState Run();
}
