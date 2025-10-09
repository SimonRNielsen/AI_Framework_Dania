using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MortensKombat
{
    public class AquireTarget : MKNode
    {
        SuperMorten baseAI;
        private readonly float dataFreshness = 2f;
        public AquireTarget(MKBlackboard blackboard, SuperMorten ai) : base(blackboard)
        {
            baseAI = ai;
        }

        public override NodeState Evaluate()
        {
            //Debug.Log("Trying to aquire taget...");
            List<EnemyData> enemies = blackboard.GetEnemies(baseAI).Where(x => Time.time - x.timestamp <= dataFreshness).ToList();
            if (enemies.Count > 0)
            {
                EnemyData targetEnemy = enemies[0];
                float distanceToTarget = float.PositiveInfinity;
                foreach (EnemyData enemy in enemies)
                {
                    float distanceToEnemy = Vector3.Distance(baseAI.transform.position, enemy.Position);
                    if (distanceToEnemy < distanceToTarget)
                    {
                        targetEnemy = enemy;
                        distanceToTarget = distanceToEnemy;
                    }
                }
                baseAI.Target = targetEnemy;
                //Debug.Log($"Target set, enemy with ID: {targetEnemy.id}");
                return NodeState.Success;
            }
            baseAI.Target = null;
            //Debug.Log("No target could be aquired: no enemies found in blackboard");
            return NodeState.Failure;
        }
    }
}
