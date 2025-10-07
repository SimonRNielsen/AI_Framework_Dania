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
    private SuperMorten mortenAI;
    

    public MoveToCP(MKBlackboard blackboard, SuperMorten mortenAI) : base(blackboard)
    {
        this.mortenAI = mortenAI;
    }

    public override NodeState Evaluate()
    {

        //var enemies = ai.GetVisibleEnemiesSnapshot();
        //if (enemies.Count > 0)
        //{

        //    ai.RefreshOrAcquireTarget();

        //    if (blackboard != null)
        //    {

        //        blackboard.SetValue(ai.MyDetectable.TeamID + enemyPosition, enemies[0].Position);
        //        blackboard.SetValue(ai.MyDetectable.TeamID + enemyTime, Time.time);

        //    }

        //    return NodeState.Success;

        //}

        return NodeState.Failure;
    }
}
