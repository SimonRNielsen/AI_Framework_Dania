using AIGame.Core;
using UnityEngine;
namespace MortensKombat
{
    public class AttackerMoveAround : MKNode
    {
        #region Fields
        private SuperMorten baseAI;
        private Vector3 controlPointPosition = AIGame.Core.ControlPoint.Instance.transform.position;
        private Vector3 destination = Vector3.zero;
        private bool teamRed = true;

        //patrol area
        private Vector3 redCornerA = new Vector3(300, 22, 0);
        private Vector3 redCornerC = new Vector3(5, 5, 0);
        private Vector3 blueCornerA = new Vector3(0, 100, 0);
        private Vector3 blueCornerC = new Vector3(0, 200, 0);
        #endregion

        #region Constructor
        public AttackerMoveAround(MKBlackboard blackboard, SuperMorten baseAI) : base(blackboard)
        {
            this.baseAI = baseAI;
            //destination = controlPointPosition;
        }
        #endregion

        #region Methods
        public override NodeState Evaluate()
        {
            Debug.Log("UWU");

            Vector3 cornerA, cornerC;

            if (IsTeamRed())
            {
                cornerA = redCornerA;
                cornerC = redCornerA;
            }
            else
            {
                cornerA = blueCornerA;
                cornerC = blueCornerC;
            }

            if (destination == Vector3.zero || Vector3.Distance(baseAI.transform.position, destination) < baseAI.ArrivalTreshold )
            {
                destination = GetRandomPoint(cornerA, cornerC, baseAI.transform.position.y);
                baseAI.MoveTo(destination);

                Debug.Log("sUCCES WUHUUU");
                return NodeState.Success;
            }


            //if (Vector3.Distance(baseAI.transform.position, destination) > baseAI.ArrivalTreshold)
            //{
            //    baseAI.MoveTo(destination);

            //    Debug.Log("MOVE RUNNING");
            //    return NodeState.Running;
            //}

            return NodeState.Failure;




        }

        public bool IsTeamRed()
        {
            Debug.Log("RED");
            return baseAI.MyDetectable.TeamID == Team.Red;
        }

        public Vector3 GetRandomPoint(Vector3 cornerA, Vector3 cornerC, float yLevel)
        {
            float minX = Mathf.Min(cornerA.x, cornerC.x);
            float maxX = Mathf.Max(cornerA.x, cornerC.x);

            float minZ = Mathf.Min(cornerA.z, cornerC.z);
            float maxZ = Mathf.Max(cornerA.z, cornerC.z);

            Debug.Log("RANDOM POINT");

            return new Vector3(Random.Range(minX, maxX), yLevel, Random.Range(minZ, maxZ));
        }
        #endregion
    }
}
