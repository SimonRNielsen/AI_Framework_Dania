using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;

public class AmIInCP : MKNode
{
    private SuperMorten supMorten;
    private ControlPoint controlPoint;
    public AmIInCP(MKBlackboard blackboard, SuperMorten mortenAI) : base(blackboard)
    {
        this.supMorten = mortenAI;
        this.controlPoint = ControlPoint.Instance;
    }

    public override NodeState Evaluate()
    {
        return NodeState.Success;
    }
}

