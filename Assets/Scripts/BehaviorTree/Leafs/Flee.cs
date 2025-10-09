using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;
using System.Linq;


public class Flee : MKNode
{
    #region Field
    private SuperMorten mortenAI;
    private Vector3 spawnPosition;
    private Vector3 fleePosition;
    private float addFlee = 5;

    #endregion

    public Flee(MKBlackboard blackboard, SuperMorten mortenAI) : base(blackboard)
    {
        this.mortenAI = mortenAI;

        spawnPosition = mortenAI.SpawnPosition;

    }

    public override NodeState Evaluate()
    {

        Vector3? closestEnemy = blackboard.GetEnemies(mortenAI).OrderBy(x => Vector3.Distance(mortenAI.transform.position, x.position)).FirstOrDefault()?.position;

        fleePosition = spawnPosition;

        if (closestEnemy.HasValue)
        {

            Vector3 enemy = closestEnemy.Value;

            if (enemy.z > mortenAI.transform.position.z)
            {
                fleePosition.z = enemy.z - addFlee;
            }
            else
            {
                fleePosition.z = enemy.z + addFlee;
            }
        }

        //throw new System.Exception(fleePosition.ToString());

        mortenAI.TargetDestination = fleePosition;

        mortenAI.FaceTarget(-mortenAI.SpawnPosition);

        mortenAI.StrafeTo(mortenAI.TargetDestination);

        return NodeState.Success;
    }

}
