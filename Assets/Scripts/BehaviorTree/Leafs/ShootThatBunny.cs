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

                //baseAI.FaceTarget(baseAI.Target.Position);

                if (baseAI.ThrowBallInDirection(baseAI.Target.Position - baseAI.transform.position, Mathf.Max(magicAngle / 2, magicAngle * (Vector3.Distance(baseAI.Target.Position, baseAI.transform.position) / baseAI.ProjectileRange))))
                    return NodeState.Success;


            }
            //Debug.Log("Shot failed: no target found");
            return NodeState.Failure;
        }

        
    }
}
