using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;

public class AmIInCP : MKNode
{
    #region Fields
    private SuperMorten supMorten;
    private ControlPoint controlPoint;
    #endregion

    #region Constructor
    public AmIInCP(MKBlackboard blackboard, SuperMorten mortenAI) : base(blackboard)
    {
        this.supMorten = mortenAI;
        this.controlPoint = ControlPoint.Instance;
    }
    #endregion

    #region Methods
    public override NodeState Evaluate()
    {
        //Sets arbitrary radius for CP
        float radiusCP = 30f;

        float distance = Vector3.Distance(supMorten.transform.position, controlPoint.transform.position);
        bool insideCP = distance < radiusCP;

        if(distance < radiusCP)
        {
            Debug.Log("Am I in CP in succes");
            return NodeState.Success;
        }

        //Out of CP
        return NodeState.Failure;

        
    }
    #endregion
}

