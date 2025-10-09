using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;
using System.Linq;

    //Bevæger sig kun i højre eller venstre hjørne

namespace MortensKombat
{

    public class AttackerMoveAroundHybrid : MKNode
    {
        #region Fields
        private SuperMorten baseAI;
        private Vector3 controlPointPosition = AIGame.Core.ControlPoint.Instance.transform.position;
        private Vector3 destination = Vector3.zero;

        private Vector3 redAreaA = new Vector3(-19, 0, 32); //5
        private Vector3 redAreaB = new Vector3(-18, 0, -32);
        private Vector3 blueAreaA = new Vector3(20, 0, 30); 
        private Vector3 blueAreaB = new Vector3(19, 0, -32);
        private float areaRadius = 15; //15//40

        private Vector3 assignedCenter;
        private bool initialized = false;

        #endregion

        #region Constructor
        public AttackerMoveAroundHybrid(MKBlackboard blackboard, SuperMorten baseAI) : base(blackboard)
        {
            this.baseAI = baseAI;
            
        }

        #endregion

        #region Methods
        

        public override NodeState Evaluate()
        {
                bool isRed = IsTeamRed();

            if (!initialized)
            {
                assignedCenter = ChooseAreaByAgentID(isRed);
                initialized = true;
                Debug.Log("$\"{(isRed ? \"Red\" : \"Blue\"");
                //Vector3 center = isRed ? redAreaA : blueAreaA;
            }
                float distance = Vector3.Distance(baseAI.transform.position, baseAI.TargetDestination);

                // Hvis vi ikke har et mål, eller vi er tæt på vores nuværende mål - vælg et nyt
                if (baseAI.TargetDestination == Vector3.zero || distance < baseAI.ArrivalTreshold)
                {
                    //center = ChooseRandomArea(isRed);
                    Vector2 offset = Random.insideUnitCircle * areaRadius;
                    Vector3 offset3D = new Vector3(offset.x, 0f, offset.y);
                    baseAI.TargetDestination = assignedCenter + offset3D;

                    baseAI.MoveTo(baseAI.TargetDestination);
                    Debug.Log($"{(isRed ? "Red" : "Blue")} unit moving to new point: {baseAI.TargetDestination}");
                }

                

                // Hvis der ikke er en aktiv path, sæt en ny destination for at genstarte bevægelsen
                if (!baseAI.HasPath())
                {
                    //center = ChooseRandomArea(isRed);
                    Vector2 offset = Random.insideUnitCircle * areaRadius;
                    Vector3 offset3D = new Vector3(offset.x, 0f, offset.y);
                    baseAI.TargetDestination = assignedCenter + offset3D;
                    baseAI.MoveTo(baseAI.TargetDestination);
                    Debug.Log("choosing new random destination");
                }

            
                return NodeState.Running; // Bliv ved med at køre
        }
        
        
            private Vector3 ChooseAreaByAgentID(bool isRed)
            {
                int id = baseAI.AgentID;

                if(isRed)
                    return (id % 2 == 0) ? redAreaA : redAreaB;
                else
                    return (id % 2 == 0) ? blueAreaA : blueAreaB;
            }

            private bool IsTeamRed()
            {
                return baseAI.MyDetectable.TeamID == Team.Red;
            }


        #endregion
    }
}
