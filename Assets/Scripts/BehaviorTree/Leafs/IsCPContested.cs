using UnityEngine;

namespace MortensKombat
{
    public class IsCPContested : MKNode
    {
        private SuperMorten baseAI;
        public IsCPContested(MKBlackboard blackboard, SuperMorten baseAI) : base(blackboard)
        {
            this.baseAI = baseAI;
        }

        public override NodeState Evaluate()
        {
            if (baseAI.EnemyTakingCP)
            {
                return NodeState.Success;
            }
            else
            {
                return NodeState.Failure;
            }

        }
    }
}
