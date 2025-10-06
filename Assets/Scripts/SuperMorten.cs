using UnityEngine;
using AIGame.Core;
using System;
using System.Collections.Generic;

namespace MortensKombat
{
    /// <summary>
    /// SuperMorten AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class SuperMorten : BaseAI
    {

        protected MKBlackboard blackboard;
        private const float SUPPLYDATA = 0.1f;
        private float timeSinceDataUpdate = SUPPLYDATA;
        private static Action<Vector3> BallDetectedVector;
        private static List<Ball> ballsHandled = new List<Ball>();
        private static bool attackerNameTaken;
        private static bool defenderNameTaken;

        /// <summary>
        /// Configure the agent's stats (speed, health, etc.).
        /// </summary>
        protected override void ConfigureStats() { }

        /// <summary>
        /// Called once when the agent starts.
        /// Use this for initialization.
        /// </summary>
        protected override void StartAI()
        {

            blackboard = MKBlackboard.GetShared(this);
            FlagEnter += SpottedFlag;
            BallDetected += IsBallArmedAndNotHandled;
            BallDetectedVector += IsBallIncoming;

        }

        private void OnDisable()
        {

            FlagEnter -= SpottedFlag;
            BallDetected -= IsBallArmedAndNotHandled;
            BallDetectedVector -= IsBallIncoming;

        }

        /// <summary>
        /// Called every frame to make decisions.
        /// Implement your AI logic here.
        /// </summary>
        protected override void ExecuteAI()
        {

            timeSinceDataUpdate += Time.deltaTime;
            if (timeSinceDataUpdate >= SUPPLYDATA)
            {

                timeSinceDataUpdate = 0f;
                SupplyData();

            }

        }


        protected virtual void SupplyData()
        {

            var enemies = GetVisibleEnemiesSnapshot();

            if (enemies.Count > 0)
                foreach (var enemy in enemies)
                    blackboard.SetValue(MyDetectable.TeamID + blackboard.enemy, enemy);

        }


        private void SpottedFlag(Team id)
        {

            if (id == MyDetectable.TeamID || blackboard.HasKey(MyDetectable.TeamID + blackboard.flag)) return;

            var flags = GetVisibleFlagsSnapShot();

            if (flags.Count > 0)
                foreach (var flag in flags)
                    blackboard.SetValue(MyDetectable.TeamID + blackboard.flag, flag);

        }


        private void IsBallArmedAndNotHandled(Ball ball)
        {

            if (!ball.Armed || ballsHandled.Contains(ball)) return;

            ballsHandled.Add(ball);

            Rigidbody rigidbody = ball.GetComponent<Rigidbody>();

            if (rigidbody != null)
                BallDetectedVector?.Invoke(rigidbody.linearVelocity);

        }


        private void IsBallIncoming(Vector3 ballRigidBodyVelocity)
        {

            if (!CanDodge()) return;



            Vector3 horizontalVelocity = new Vector3(ballRigidBodyVelocity.x, 0f, ballRigidBodyVelocity.z);
            horizontalVelocity.Normalize();
            StartDodge(UnityEngine.Random.value < 0.5f ? new Vector3(horizontalVelocity.z, 0f, -horizontalVelocity.x) : new Vector3(-horizontalVelocity.z, 0f, horizontalVelocity.x));

        }

        protected override string SetName()
        {

            switch (ToString())
            {
                case "Scout":
                    return "Undercover Morten";
                case "Defender":
                    defenderNameTaken = defenderNameTaken ? false : true;
                    return defenderNameTaken ? "Crusader Morten" : "Munke Morten";
                case "Attacker":
                    attackerNameTaken = attackerNameTaken ? false : true;
                    return attackerNameTaken ? "Holy Morten" : "Martin";
                default:
                    return "SuperMorten";
            }

        }

    }
}