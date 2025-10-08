using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;

//set en distancetoenemy, hvis den 

public class EnemyInrange : MKNode
{
    #region Fields
    private SuperMorten supMorten;
    ControlPoint controlPoint = ControlPoint.Instance;

    #endregion

    #region Constructor
    public EnemyInrange(MKBlackboard blackboard, SuperMorten mortenAI) : base(blackboard)
    {
        this.supMorten = mortenAI;

    }
    #endregion

    #region Methods
    public override NodeState Evaluate()
    {
        //Vector3? closestEnemy = blackboard.GetEnemies(supMorten)
        //    .OrderBy(x => Vector3.Distance(mortenAI.transform.position, x.position))
        //    .FirstOrDefault()?.position;


        throw new System.NotImplementedException();
    }
    #endregion
}
