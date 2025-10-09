using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;
using System.Linq;

//Bev�ger sig mellem h�jre eller venstre hj�rne


namespace MortensKombat
{
    public class AttackerMoveAround : MKNode
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


        #endregion

        #region Constructor
        public AttackerMoveAround(MKBlackboard blackboard, SuperMorten baseAI) : base(blackboard)
        {
            this.baseAI = baseAI;
            
        }

        #endregion

        #region Methods
        


            public override NodeState Evaluate()
            {
            

            bool isRed = IsTeamRed();
            Vector3 center = isRed ? redAreaA : blueAreaA;

            float distance = Vector3.Distance(baseAI.transform.position, baseAI.TargetDestination);

            // Hvis vi ikke har et m�l, eller vi er t�t p� vores nuv�rende m�l - v�lg et nyt
            if (baseAI.TargetDestination == Vector3.zero || distance < baseAI.ArrivalTreshold)
            {
                center = ChooseRandomArea(isRed);
                Vector2 offset = Random.insideUnitCircle * areaRadius;
                Vector3 offset3D = new Vector3(offset.x, 0f, offset.y);
                baseAI.TargetDestination = center + offset3D;

                baseAI.MoveTo(baseAI.TargetDestination);
                Debug.Log($"{(isRed ? "Red" : "Blue")} unit moving to new point: {baseAI.TargetDestination}");
            }

            // Forts�t med at bev�ge dig mod destinationen
            if (baseAI.HasPath() && !baseAI.IsStopped())
            {
                return NodeState.Running;
            }

            // Hvis der ikke er en aktiv path, s�t en ny destination for at genstarte bev�gelsen
            if (!baseAI.HasPath())
            {
                center = ChooseRandomArea(isRed);
                Vector2 offset = Random.insideUnitCircle * areaRadius;
                Vector3 offset3D = new Vector3(offset.x, 0f, offset.y);
                baseAI.TargetDestination = center + offset3D;
                baseAI.MoveTo(baseAI.TargetDestination);
                Debug.Log("choosing new random destination");
            }

            return NodeState.Running; // Bliv ved med at k�re
        }

        private Vector3 ChooseRandomArea(bool isRed)
        {
            bool chooseFirst = Random.value > 0.5f;
            if(isRed)
                return chooseFirst ? redAreaA : redAreaB;
            else
                return chooseFirst ? blueAreaA : blueAreaB;
        }

        private bool IsTeamRed()
        {
            return baseAI.MyDetectable.TeamID == Team.Red;
            }


        #endregion
    }
}
