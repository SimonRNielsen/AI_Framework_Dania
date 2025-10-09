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
    private float fleeX = 5f;

    #endregion

    public Flee(MKBlackboard blackboard, SuperMorten mortenAI) : base(blackboard)
    {
        this.mortenAI = mortenAI;

        spawnPosition = mortenAI.SpawnPosition;

    }

    public override NodeState Evaluate()
    {

        Vector3? closestEnemy = blackboard.GetEnemies(mortenAI).OrderBy(x => Vector3.Distance(mortenAI.transform.position, x.Position)).FirstOrDefault()?.Position;

        fleePosition = spawnPosition;

        //Moving away from the enemy
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

        //Walking back to the spawn point
        if (mortenAI.transform.position.x > 0)
        {
            fleePosition.x = mortenAI.transform.position.x + fleeX;
        }
        else
        {
            fleePosition.x = mortenAI.transform.position.x - fleeX;
        }


        mortenAI.TargetDestination = fleePosition;

        mortenAI.FaceTarget(-mortenAI.SpawnPosition);

        mortenAI.StrafeTo(mortenAI.TargetDestination);

        return NodeState.Success;
    }

}
