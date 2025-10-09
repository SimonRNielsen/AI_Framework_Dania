using AIGame.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MortensKombat;
using System.Linq;

//Bevæger sig mellem højre eller venstre hjørne


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



        ////patrol area
        //private Vector3 redCornerA = new Vector3(300, 22, 0);
        //private Vector3 redCornerC = new Vector3(5, 5, 0);
        //private Vector3 blueCornerA = new Vector3(0, 100, 0);
        //private Vector3 blueCornerC = new Vector3(0, 200, 0);
        #endregion

        #region Constructor
        public AttackerMoveAround(MKBlackboard blackboard, SuperMorten baseAI) : base(blackboard)
        {
            this.baseAI = baseAI;
            
        }

        #endregion

        #region Methods
        //    public override NodeState Evaluate()
        //    {
        //        Debug.Log("UWU");

        //        Vector3 cornerA, cornerC;

        //        if (IsTeamRed())
        //        {
        //            cornerA = redCornerA;
        //            cornerC = redCornerA;
        //        }
        //        else
        //        {
        //            cornerA = blueCornerA;
        //            cornerC = blueCornerC;
        //        }

        //        if (destination == Vector3.zero || Vector3.Distance(baseAI.transform.position, destination) < baseAI.ArrivalTreshold )
        //        {
        //            destination = GetRandomPoint(cornerA, cornerC, baseAI.transform.position.y);
        //            baseAI.MoveTo(destination);

        //            Debug.Log("sUCCES WUHUUU");
        //            return NodeState.Success;
        //        }


        //        //if (Vector3.Distance(baseAI.transform.position, destination) > baseAI.ArrivalTreshold)
        //        //{
        //        //    baseAI.MoveTo(destination);

        //        //    Debug.Log("MOVE RUNNING");
        //        //    return NodeState.Running;
        //        //}

        //        return NodeState.Failure;




        //    }

        //    public bool IsTeamRed()
        //    {
        //        Debug.Log("RED");
        //        return baseAI.MyDetectable.TeamID == Team.Red;
        //    }

        //    public Vector3 GetRandomPoint(Vector3 cornerA, Vector3 cornerC, float yLevel)
        //    {
        //        float minX = Mathf.Min(cornerA.x, cornerC.x);
        //        float maxX = Mathf.Max(cornerA.x, cornerC.x);

        //        float minZ = Mathf.Min(cornerA.z, cornerC.z);
        //        float maxZ = Mathf.Max(cornerA.z, cornerC.z);

        //        Debug.Log("RANDOM POINT");

        //        return new Vector3(Random.Range(minX, maxX), yLevel, Random.Range(minZ, maxZ));
        //    }


            public override NodeState Evaluate()
            {
            ////    bool isRed = IsTeamRed();
            ////    Vector3 center = isRed ? redAreaA : blueAreaA;

            ////    Vector2 offset = Random.insideUnitCircle * 20f; //the number is radius
            ////    Vector3 offset3D = new Vector3(offset.x, 0f, offset.y);
            ////    baseAI.TargetDestination = center + offset3D + new Vector3(5, 0, 5);
            ////    baseAI.MoveTo(baseAI.TargetDestination);



            ////    float distance = Vector3.Distance(baseAI.transform.position, baseAI.TargetDestination);

            ////    if (distance < /*areaRadius*/ baseAI.ArrivalTreshold)
            ////    {
            ////        Debug.Log("WUHU SUCCES");
            ////        return NodeState.Success;
            ////    }

            ////    //if (baseAI.HasPath() && !baseAI.IsStopped())
            ////    //{
            ////    //    Debug.Log("WUHU RUNNING");

            ////    //    return NodeState.Running;
            ////    //}

            ////    Debug.Log("WUHU FAIL");

            ////    return NodeState.Failure;




            ////        //throw new System.NotImplementedException();
            ////    }

            ////private bool IsTeamRed()
            ////{
            ////    return baseAI.MyDetectable.TeamID == Team.Red;
            ////
            ///



            bool isRed = IsTeamRed();
            Vector3 center = isRed ? redAreaA : blueAreaA;

            float distance = Vector3.Distance(baseAI.transform.position, baseAI.TargetDestination);

            // Hvis vi ikke har et mål, eller vi er tæt på vores nuværende mål - vælg et nyt
            if (baseAI.TargetDestination == Vector3.zero || distance < baseAI.ArrivalTreshold)
            {
                center = ChooseRandomArea(isRed);
                Vector2 offset = Random.insideUnitCircle * areaRadius;
                Vector3 offset3D = new Vector3(offset.x, 0f, offset.y);
                baseAI.TargetDestination = center + offset3D;

                baseAI.MoveTo(baseAI.TargetDestination);
                Debug.Log($"{(isRed ? "Red" : "Blue")} unit moving to new point: {baseAI.TargetDestination}");
            }

            // Fortsæt med at bevæge dig mod destinationen
            if (baseAI.HasPath() && !baseAI.IsStopped())
            {
                return NodeState.Running;
            }

            // Hvis der ikke er en aktiv path, sæt en ny destination for at genstarte bevægelsen
            if (!baseAI.HasPath())
            {
                center = ChooseRandomArea(isRed);
                Vector2 offset = Random.insideUnitCircle * areaRadius;
                Vector3 offset3D = new Vector3(offset.x, 0f, offset.y);
                baseAI.TargetDestination = center + offset3D;
                baseAI.MoveTo(baseAI.TargetDestination);
                Debug.Log("choosing new random destination");
            }

            return NodeState.Running; // Bliv ved med at køre
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
