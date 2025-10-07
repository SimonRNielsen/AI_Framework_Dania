using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;

// baseai.position - find egen position

//Set destination 0,0 CP

//Find path

//Move

//Check - succes


public class MoveToCP : MKNode
{
    private SuperMorten supMorten;
    ControlPoint controlPoint = ControlPoint.Instance;


    public MoveToCP(MKBlackboard blackboard, SuperMorten mortenAI) : base(blackboard)
    {
        this.supMorten = mortenAI;
        this.controlPoint = ControlPoint.Instance;
    }



    public override NodeState Evaluate()
    {
        //Starting NavMesh agent
        if (!supMorten.IsStopped())
        {
            supMorten.SetStopped(false);
        }

        //Agent bevæger sig til Control point
        supMorten.MoveTo(controlPoint.transform.position);

            if (supMorten.transform.position == controlPoint.transform.position)
            {
                return NodeState.Success;
            }

            if (supMorten.transform.position != controlPoint.transform.position)
            {
                return NodeState.Running;
            }

        
        
        if (supMorten.IsStopped())
        {
            return NodeState.Failure;
        }



    }
}
