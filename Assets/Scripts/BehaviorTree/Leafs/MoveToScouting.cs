using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;

public class MoveToScouting : MKNode
{
    #region Field
    private BaseAI mortenAI;
    private Vector3 startPosition;
    private Vector3 aPosition;
    private Vector3 bPosition;

    #endregion

    #region Constructor
    public MoveToScouting(MKBlackboard blackboard, SuperMorten superMorten) : base(blackboard)
    {
        this.mortenAI = superMorten;
    }

    #endregion

    #region Method

    public override NodeState Evaluate()
    {
        throw new System.NotImplementedException();

        
    }

    private void Positions()
    {
        startPosition = mortenAI.transform.position;

        var controlPoint = ControlPoint.Instance;

        Vector3 cpPosition = controlPoint.transform.position;
    }

    #endregion

    private bool HasReachedDestination()
    {

        if (mortenAI.GetRemainingDistance() <= ARRIVAL_THRESHOLD)
        {
            return true;
        }
        else if (!mortenAI.IsPathPending() && !mortenAI.HasPath() && Vector3.Distance(mortenAI.transform.position, currentDestination) <= ARRIVAL_THRESHOLD)
        {
            return true;
        }

        return false;

    }
}
