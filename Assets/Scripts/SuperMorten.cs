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
        private readonly float SUPPLYDATA = 0.1f;                           //Timer for renewing data
        private float timeSinceDataUpdate;
        private static Action<Vector3, Vector3, Team> BallDetectedVector;   //Transmits data to teammembers
        private static List<Ball> ballsHandled = new List<Ball>();          //Tracks which balls have been handled
        private static bool attackerNameTaken;
        private static bool defenderNameTaken;

        /// <summary>
        /// Configure the agent's stats (speed, health, etc.).
        /// </summary>
        protected override void ConfigureStats() { }    //Allocated in subclasses

        /// <summary>
        /// Called once when the agent starts.
        /// Use this for initialization.
        /// </summary>
        protected override void StartAI()
        {

            blackboard = MKBlackboard.GetShared(this);  //Connects agent to shared blackboard
            FlagEnter += SpottedFlag;                   //Records data on enemy flag
            BallDetected += IsBallArmedAndNotHandled;   //Dataprocessing method that invokes BallDetectedVector if ball is dangerous
            BallDetectedVector += IsBallIncoming;       //Transmits data to all teammembers

            timeSinceDataUpdate = SUPPLYDATA;           //Instant update on Execute

        }

        private void OnDisable() //Unsubscribe events
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
            if (timeSinceDataUpdate >= SUPPLYDATA) //Loops update of data on timer
            {

                timeSinceDataUpdate = 0f;
                SupplyData();

            }

        }

        /// <summary>
        /// Updates data on enemies within visual range on Blackboard
        /// </summary>
        protected virtual void SupplyData()
        {

            var enemies = GetVisibleEnemiesSnapshot();

            if (enemies.Count > 0)
                foreach (var enemy in enemies)
                    blackboard.SetValue(MyDetectable.TeamID + blackboard.enemy, enemy);

        }

        /// <summary>
        /// Stores location of enemy flag in Blackboard for interception
        /// </summary>
        /// <param name="id">Which team the flag belongs to</param>
        private void SpottedFlag(Team id)
        {

            if (id == MyDetectable.TeamID || blackboard.HasKey(MyDetectable.TeamID + blackboard.flag)) return; //Early return if flag was already found

            var flags = GetVisibleFlagsSnapShot();

            if (flags.Count > 0)
                foreach (var flag in flags)
                    blackboard.SetValue(MyDetectable.TeamID + blackboard.flag, flag);

        }

        /// <summary>
        /// Checks detected ball for conditions
        /// </summary>
        /// <param name="ball">Ball that has been detected</param>
        private void IsBallArmedAndNotHandled(Ball ball)
        {

            if (!ball.Armed || ballsHandled.Contains(ball)) return; //Early return if ball isn't dangerous or already been processed

            ballsHandled.Add(ball); //Flags ball as processed

            Rigidbody rigidbody = ball.GetComponent<Rigidbody>();

            if (rigidbody != null)
                BallDetectedVector?.Invoke(rigidbody.linearVelocity, rigidbody.position, MyDetectable.TeamID); //Transmits relevant data to teammembers

        }

        /// <summary>
        /// Supplies relevant data to all subscribers to check if they need to initiate a dodge, and if yes, starts dodging
        /// </summary>
        /// <param name="ballRigidbodyVelocity">Velocity of triggering ball</param>
        /// <param name="ballRigidbodyPosition">Position of triggering ball</param>
        /// <param name="id">Team that needs to beware</param>
        private void IsBallIncoming(Vector3 ballRigidbodyVelocity, Vector3 ballRigidbodyPosition, Team id)
        {

            if (!CanDodge() || id != MyDetectable.TeamID) return; //Early return if unable to dodge (or unnecessary because it's own teams ball)



            Vector3 horizontalVelocity = new Vector3(ballRigidbodyVelocity.x, 0f, ballRigidbodyVelocity.z); //Get only horizontal values
            horizontalVelocity.Normalize(); //Normalize for direction
            StartDodge(UnityEngine.Random.value < 0.5f ? new Vector3(horizontalVelocity.z, 0f, -horizontalVelocity.x) : new Vector3(-horizontalVelocity.z, 0f, horizontalVelocity.x)); //Randomize dodge to direct left or right of incoming ball

        }

        //Override of SetName to change agents name into class predefined ones
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

        public override string ToString() => "SuperMorten";

    }
}