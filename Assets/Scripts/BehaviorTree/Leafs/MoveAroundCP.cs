using UnityEngine;
namespace MortensKombat
{
    public class MoveAroundCP : MKNode
    {
        private SuperMorten baseAI;
        private Vector3 controlPointPosition = AIGame.Core.ControlPoint.Instance.transform.position;
        private Vector3 destination;
        public MoveAroundCP(MKBlackboard blackboard, SuperMorten baseAI) : base(blackboard)
        {
            this.baseAI = baseAI;
            destination = controlPointPosition;
        }

        public override NodeState Evaluate()
        {
            if (destination == controlPointPosition)
            {
                destination = new Vector3(Random.Range(controlPointPosition.x - 30, controlPointPosition.x + 30), baseAI.transform.position.y, Random.Range(controlPointPosition.y-35, controlPointPosition.y + 23));
                Debug.Log($"New destination set to {destination}");
            }
            if (Vector3.Distance(baseAI.transform.position, destination) > baseAI.ArrivalTreshold)
            {
                Debug.Log("Moving around inside cp");
                baseAI.MoveTo(destination);
                return NodeState.Running;
            }
            else
            {
                Debug.Log($"Destiantion: {destination} reached!");
                destination = controlPointPosition;
                return NodeState.Success;
            }
        }
    }
}
