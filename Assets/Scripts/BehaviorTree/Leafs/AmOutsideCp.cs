using AIGame.Core;
using UnityEngine;

namespace MortensKombat
{
    public class AmOutsideCp : MKNode
    {
        private SuperMorten baseAI;
        private ControlPoint controlPoint;
        private float radiusCP = 45f; //The radius around cp, that counts as "in" CP. 
        public AmOutsideCp(MKBlackboard blackboard, SuperMorten baseAI) : base(blackboard)
        {
            this.baseAI = baseAI;
            this.controlPoint = ControlPoint.Instance;
        }

        public override NodeState Evaluate()
        {
            float distance = Vector3.Distance(baseAI.transform.position, controlPoint.transform.position);
            bool outsideCP = distance > radiusCP;

            if (outsideCP)
            {
                Debug.Log("Am outside CP is succes");
                return NodeState.Success;
            }

            //Not outside of CP
            Debug.Log("Not Outside CP");
            return NodeState.Failure;

        }
    }
}
