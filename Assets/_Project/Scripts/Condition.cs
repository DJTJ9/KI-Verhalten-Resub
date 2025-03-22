using System;
using UnityEngine;

public class Condition : Node
{
   private readonly Func<bool> condition;

   public Condition(Func<bool> condition) {
      this.condition = condition;
   }

   public override NodeState Run() => condition() ? NodeState.Success : NodeState.Failure;
}
