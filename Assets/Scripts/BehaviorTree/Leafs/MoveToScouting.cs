using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToScouting : MKNode
{
    #region Field
    private BaseAI baseAI;

    #endregion

    public MoveToScouting(MKBlackboard blackboard, BaseAI baseAI) : base(blackboard)
    {
        this.baseAI = baseAI;

    }

    public override NodeState Evaluate()
    {
        throw new System.NotImplementedException();
    }
}
