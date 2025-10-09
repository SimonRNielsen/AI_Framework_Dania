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

    #endregion

    public Flee(MKBlackboard blackboard, SuperMorten mortenAI) : base(blackboard)
    {
        this.mortenAI = mortenAI;

        this.spawnPosition = mortenAI.SpawnPosition;
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
                fleePosition.x = enemy.x + 60;
            }
            else
            {
                fleePosition.x = enemy.x - 60;
            }
        }

            //throw new System.Exception(fleePosition.ToString());

        mortenAI.TargetDestination = fleePosition;

        //mortenAI.FaceTarget(-spawnPosition);

        //mortenAI.StrafeTo(mortenAI.TargetDestination);

        mortenAI.MoveTo(spawnPosition);

        return NodeState.Success;
    }

}
