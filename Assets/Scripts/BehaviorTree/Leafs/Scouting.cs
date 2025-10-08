using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;

public class Scouting : MKNode
{
    #region Field
    private SuperMorten mortenAI;

    #endregion

    public Scouting(MKBlackboard blackboard) : base(blackboard)
    {
    }

    public override NodeState Evaluate()
    {
        throw new System.NotImplementedException();
    }
}
