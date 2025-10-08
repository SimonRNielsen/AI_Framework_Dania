using UnityEngine;
namespace MortensKombat
{
    public class MoveAroundCP : MKNode
    {
        private SuperMorten baseAI;
        private Vector3 destination = Vector3.zero;
        private Vector3 controlPointPosition = AIGame.Core.ControlPoint.Instance.transform.position;
        public MoveAroundCP(MKBlackboard blackboard, SuperMorten baseAI) : base(blackboard)
        {
            this.baseAI = baseAI;
        }

        public override NodeState Evaluate()
        {
            if (destination == controlPointPosition)
            {
                destination = new Vector3(Random.Range(controlPointPosition.x - 60, controlPointPosition.x + 60), baseAI.transform.position.y, Random.Range(controlPointPosition.y - 60, controlPointPosition.y + 60));
            }
            if (Vector3.Distance(baseAI.transform.position, destination) > 0.1f)
            {
                baseAI.MoveTo(destination);
                return NodeState.Running;
            }
            else
            {
                destination = controlPointPosition;
                return NodeState.Success;
            }
        }
    }
}
