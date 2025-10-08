using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;
using System.Linq;

public class MoveToScouting : MKNode
{
    #region Field
    private SuperMorten mortenAI;
    private List<Vector3> positions = new List<Vector3>();
    private int numberPosition = 0;
    private Vector3 cpPosition;
    private float offset = 0.5f;
    private float enemyRange = 2f;

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

        Vector3? closestEnemy = blackboard.GetEnemies(mortenAI)
            .OrderBy(x => Vector3.Distance(mortenAI.transform.position, x.position))
            .FirstOrDefault()?.position;

        if (closestEnemy.HasValue)
        {
            if (closestEnemy != Vector3.zero)
            {
                if (Vector3.Distance(closestEnemy.Value, mortenAI.transform.position) < enemyRange)
                {
                    return NodeState.Failure;
                }
            }
        }

        //Staring the NavMesh agent
        if (!mortenAI.IsStopped())
        {
            mortenAI.SetStopped(false);
        }

        //If empty get the scout positions 
        if (positions.Count == 0)
        {
            Positions();
        }

        //If there isn't any target destination set the destination to first in positions
        if (mortenAI.TargetDestination == null)
        {
            float standing = mortenAI.transform.position.z;

            if (Mathf.Abs(positions[0].z - standing) < Mathf.Abs(positions[1].z - standing))
            {
                mortenAI.TargetDestination = positions[0];
            }
            else
            {
                mortenAI.TargetDestination = positions[1];
            }

            mortenAI.FaceTarget(cpPosition);
        }

        //If the target destination has been reached set the target destination to the next position
        if (HasReachedDestination())
        {
            //New target destination
            mortenAI.TargetDestination = positions[numberPosition];

            numberPosition++;

            //If numberPosition is greater than positions count numberPosition will be 0 again
            if (numberPosition > positions.Count - 1)
            {
                numberPosition = 0;
            }

            //New target
            mortenAI.MoveTo(mortenAI.TargetDestination);

            return NodeState.Success;
        }

        return NodeState.Running;
    }

    /// <summary>
    /// Setting the scout position
    /// </summary>
    private void Positions()
    {
        //Getting the start position
        Vector3 startPosition = mortenAI.transform.position;

        //Getting the control point position
        var controlPoint = ControlPoint.Instance;
        cpPosition = controlPoint.transform.position;

        //midt x aksen
        float xAksis = startPosition.x * offset;
        //z aksen
        float zAksis = 3f;

        positions.Add(new Vector3(xAksis, startPosition.y, zAksis));
        positions.Add(new Vector3(xAksis, startPosition.y, -zAksis));
    }

    #endregion

    /// <summary>
    /// Returning a bool whether or not the target has been reached
    /// </summary>
    /// <returns></returns>
    private bool HasReachedDestination()
    {
        if (mortenAI.GetRemainingDistance() <= mortenAI.ArrivalTreshold)
        {
            return true;
        }
        else if (!mortenAI.IsPathPending() && !mortenAI.HasPath() && Vector3.Distance(mortenAI.transform.position, mortenAI.TargetDestination) <= mortenAI.ArrivalTreshold)
        {
            return true;
        }

        return false;

    }
}
