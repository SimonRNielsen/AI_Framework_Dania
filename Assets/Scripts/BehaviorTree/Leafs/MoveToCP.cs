using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;


public class MoveToCP : MKNode
{
    #region Fields
    private SuperMorten supMorten;
    ControlPoint controlPoint = ControlPoint.Instance;
    private float CPArrivalThreshold = 10f;
    #endregion

    #region Constructor
    public MoveToCP(MKBlackboard blackboard, SuperMorten mortenAI) : base(blackboard)
    {
        this.supMorten = mortenAI;
        this.controlPoint = ControlPoint.Instance;
    }
    #endregion



    #region Methods
    public override NodeState Evaluate()
    {
        Debug.Log($"{supMorten.name} moving toward ControlPoint");


        //Checking for Control Point spawn
        if (controlPoint == null)
        {
            Debug.Log("Missing control point");
            return NodeState.Failure;
        }

        //Starting NavMesh agent
        //if (supMorten.IsStopped())
        //{
        //  //  supMorten.SetStopped(false);
        //}


        //Sets target til Controlpoint (+x so we don't crash with the other team at 0,0) - Agent moves to Control point


        ////Random offset can also be applied instead of simple locked offset
        //Vector2 randomOffset = Random.insideUnitCircle * 10f; //the number is radius
        //Vector3 offset3D = new Vector3(randomOffset.x, 0f, randomOffset.y);
        //supMorten.TargetDestination = controlPoint.transform.position + offset3D;


        //Simple locked offset
        //supMorten.TargetDestination = controlPoint.transform.position + new Vector3(15f, 0f, 0f);
        supMorten.TargetDestination = controlPoint.transform.position;
        supMorten.MoveTo(supMorten.TargetDestination);

        //Calculates distance between agent and control point
        float distance = Vector3.Distance(supMorten.transform.position, supMorten.TargetDestination);

        //Agent arrives at CP
        if (distance < CPArrivalThreshold)
        {
            Debug.Log("MoveToCP succes");
            return NodeState.Success;
        }

        //Agent still moving, returns running
        if (supMorten.HasPath() && !supMorten.IsStopped())
        {
            return NodeState.Running;
        }

        //If it's interrupted it fails
        return NodeState.Failure;



    }
    #endregion
}
