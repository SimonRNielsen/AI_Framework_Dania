using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;
using System.Linq;

//set en distancetoenemy, hvis den 

public class EnemyInrange : MKNode
{
    #region Fields
    private SuperMorten supMorten;
    ControlPoint controlPoint = ControlPoint.Instance;
    private List<Vector3> positions = new List<Vector3>();
    private float enemyRange = 40f;


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

        //Finding closests enemy from blackboard
        Vector3? closestEnemy = blackboard.GetEnemies(supMorten)
            .OrderBy(x => Vector3.Distance(supMorten.transform.position, x.position))
            .FirstOrDefault()?.position;

        if (closestEnemy.HasValue && closestEnemy != Vector3.zero)
        {
            
                if (Vector3.Distance(closestEnemy.Value, supMorten.transform.position) < enemyRange)
                {
                
                    Debug.Log("IN RANGE SUCCESS");
                    return NodeState.Success;

                }
           
        }
        Debug.Log("IN RANGE FAIL");
        return NodeState.Failure;
    }
    #endregion
}
