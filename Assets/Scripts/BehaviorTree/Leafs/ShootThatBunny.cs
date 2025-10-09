using UnityEngine;
using AIGame.Core;
using System.Collections.Generic;

namespace MortensKombat
{
    public class ShootThatBunny : MKNode
    {
        private SuperMorten baseAI;
        private readonly float magicAngle = 16f;

        public ShootThatBunny(MKBlackboard blackboard, SuperMorten ai) : base(blackboard)
        {
            baseAI = ai;
        }

        public override NodeState Evaluate()
        {
            //Debug.Log("Trying to shoot at target");
            if (baseAI.Target != null)
            {

                float distanceToEnemyPosition = Vector3.Distance(baseAI.Target.Position, baseAI.transform.position);
                float movingTowards = 0;
                if (baseAI.Target.OldPosition != Vector3.zero && distanceToEnemyPosition < Vector3.Distance(baseAI.transform.position, baseAI.Target.OldPosition))
                    movingTowards = -3;

                if (baseAI.ThrowBallInDirection(baseAI.Target.Position - baseAI.transform.position, Mathf.Max((magicAngle / 2) + movingTowards, magicAngle * (distanceToEnemyPosition / baseAI.ProjectileRange) + movingTowards)))
                    return NodeState.Success;


            }
            //Debug.Log("Shot failed: no target found");
            return NodeState.Failure;
        }


    }
}
