using UnityEngine;
using AIGame.Core;
using System.Collections.Generic;

namespace MortensKombat
{
    public class ShootThatBunny : MKNode
    {
        private SuperMorten baseAI;
        public ShootThatBunny(MKBlackboard blackboard, SuperMorten ai) : base(blackboard)
        {
            baseAI = ai; 
        }

        public override NodeState Evaluate()
        {
            Debug.Log("Trying to shoot at target");
            if (baseAI.Target != null)
            {
                //Vector3 direction = (baseAI.Target.position - baseAI.transform.position);
                baseAI.FaceTarget(baseAI.Target.position);

                baseAI.ThrowBallAt(baseAI.Target.enemy);
                //Debug.Log("Ball thrown in direction: " + direction);
                return NodeState.Success;
            }
            Debug.Log("Shot failed: no target found");
            return NodeState.Failure;
        }
    }
}
