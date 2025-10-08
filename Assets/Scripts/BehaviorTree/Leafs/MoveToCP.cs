using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;


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
        Debug.Log($"{supMorten.name} moving toward ControlPoint");


        ////Checking for Control Point spawn
        //if (supMorten == null ||controlPoint == null)
        //{
        //    Debug.Log("Missing control point");
        //    return NodeState.Failure;
        //}

        ////Starting NavMesh agent
        //if (supMorten.IsStopped())
        //{
        //    supMorten.SetStopped(false);
        //}


        //Agent moves to Control point
        supMorten.TargetDestination = controlPoint.transform.position;
        supMorten.MoveTo(supMorten.TargetDestination);

        //Calculates distance between agent and control point
        float distance = Vector3.Distance(supMorten.transform.position, supMorten.TargetDestination);

        //Agent arrives at CP
        if (distance < supMorten.ArrivalTreshold)
        {
            return NodeState.Success;
        }

        ////Agent still moving, returns running
        //if (supMorten.HasPath() && !supMorten.IsStopped())
        //{
        //    return NodeState.Running;
        //}

        //If it's interrupted it fails
        return NodeState.Failure;
        



    }
}
