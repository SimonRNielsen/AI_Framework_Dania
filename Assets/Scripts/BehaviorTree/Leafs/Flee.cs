using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;

public class Flee : MKNode
{
    #region Field
    private SuperMorten mortenAI;
    private Vector3 spawnPosition;

    #endregion

    public Flee(MKBlackboard blackboard, SuperMorten mortenAI) : base(blackboard)
    {
        this.mortenAI = mortenAI;

        this.spawnPosition = mortenAI.SpawnPosition;
    }

    public override NodeState Evaluate()
    {
        mortenAI.TargetDestination = spawnPosition;

        mortenAI.FaceTarget(-spawnPosition);

        mortenAI.StrafeTo(spawnPosition);

        return NodeState.Success;
    }

}
