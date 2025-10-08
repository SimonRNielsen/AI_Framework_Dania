using AIGame.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

namespace MortensKombat
{
    /// <summary>
    /// SuperMorten AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class SuperMorten : BaseAI
    {
        #region Fields
        protected MKBlackboard blackboard;
        private readonly float SUPPLYDATAINTERVAL = 0.5f;                   //Timer for renewing data
        private readonly float INCOMINGDOT = 0.98f;                         //Dot value for close to directly towards this
        private static readonly float CLEARDATAINTERVAL = 1f;
        private static float lastTimeDataCleaned;
        private float lastDataStoredAt;
        private static Action<Vector3, Vector3, Team> BallDetectedVector;   //Transmits data to teammembers
        private static List<Ball> ballsHandled = new List<Ball>();          //Tracks which balls have been handled
        private static bool attackerNameTaken;
        private static bool defenderNameTaken;
        private readonly float arrivalTreshold = 1.5f;
        private Vector3 targetDestination;
        protected MKTree behaviourTree;
        private Vector3 spawnPosition = Vector3.zero;                       //Spawn position for specific position needs to be set at the under class in construction
        #endregion

        #region Properties
        public EnemyData Target { get; set; }
        public Vector3 TargetDestination { get => targetDestination; set => targetDestination = value; }
        public float ArrivalTreshold { get => arrivalTreshold; }
        public Vector3 SpawnPosition { get => spawnPosition; set => spawnPosition = value; }

        #endregion

        #region Methods


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
            EnemyEnterVision += SupplyData;             //Immediately transmits data to blackboard

        }

        //private void OnDisable() //Unsubscribe events
        //{

        //    FlagEnter -= SpottedFlag;
        //    BallDetected -= IsBallArmedAndNotHandled;
        //    BallDetectedVector -= IsBallIncoming;

        //}

        /// <summary>
        /// Called every frame to make decisions.
        /// Implement your AI logic here.
        /// </summary>
        protected override void ExecuteAI()
        {

            if (Time.time - lastDataStoredAt >= SUPPLYDATAINTERVAL) //Loops update of data on timer
                SupplyData();

            if (behaviourTree != null)
                behaviourTree.Tick();

            if (Time.time - lastTimeDataCleaned >= CLEARDATAINTERVAL)
                lastTimeDataCleaned = blackboard.RemoveObsoleteData();

        }

        /// <summary>
        /// Updates data on enemies within visual range on Blackboard
        /// </summary>
        protected virtual void SupplyData()
        {

            var enemies = GetVisibleEnemiesSnapshot();

            if (enemies.Count > 0)
                foreach (PerceivedAgent enemy in enemies)
                    blackboard.SetValue(MyDetectable.TeamID + blackboard.enemy, enemy);

            lastDataStoredAt = Time.time;

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
                foreach (PerceivedFlag flag in flags)
                    blackboard.SetValue(MyDetectable.TeamID + blackboard.flag, flag);

        }

        /// <summary>
        /// Checks detected ball for conditions
        /// </summary>
        /// <param name="ball">Ball that has been detected</param>
        private void IsBallArmedAndNotHandled(Ball ball)
        {

            if (!ball.Armed || ballsHandled.Contains(ball)) return; //Early return if ball isn't dangerous or already been processed

            Rigidbody rigidbody = ball.GetComponent<Rigidbody>();

            ballsHandled.Add(ball); //Flags ball as processed

            BallDetectedVector?.Invoke(rigidbody.linearVelocity, ball.transform.position, MyDetectable.TeamID); //Transmits relevant data to teammembers

        }

        /// <summary>
        /// Supplies relevant data to all subscribers to check if they need to initiate a dodge, and if yes, starts dodging
        /// </summary>
        /// <param name="ballVelocity">Velocity of triggering ball</param>
        /// <param name="ballOrigin">Position of thrower</param>
        /// <param name="id">Team that needs to beware</param>
        private void IsBallIncoming(Vector3 ballVelocity, Vector3 ballOrigin, Team id)
        {

            if (!CanDodge() || id != MyDetectable.TeamID) return;                                                                                                                       //Early return if unable to dodge (or unnecessary because it's own teams ball)

            Vector3 fromOriginToThis = (transform.position - ballOrigin).normalized;                                                                                                    //Calculate velocity normalized compared to this
            float dot = Vector3.Dot(ballVelocity.normalized, fromOriginToThis);                                                                                                                    //Determine Dot value of direction required to hit this, and balls direction

            //Debug.LogWarning($"Dot value for {MyName} was: {dot}"); //Debugging and Testing

            if (dot < INCOMINGDOT) return;                                                                                                                                              //Early return if ball not deemed to hit

            Vector3 horizontalVelocity = new Vector3(ballVelocity.x, 0f, ballVelocity.z);                                                                                               //Get only horizontal values
            horizontalVelocity.Normalize();                                                                                                                                             //Normalize for direction
            StartDodge(UnityEngine.Random.value < 0.5f ? new Vector3(horizontalVelocity.z, 0f, -horizontalVelocity.x) : new Vector3(-horizontalVelocity.z, 0f, horizontalVelocity.x));  //Randomize dodge to direct left or right of incoming ball

        }

        //Override of SetName to change agents name into class predefined ones
        protected override string SetName()
        {

            switch (ToString())
            {
                case "Scout":
                    return "Undercover Morten";
                case "Defender":
                    defenderNameTaken = !defenderNameTaken;
                    return defenderNameTaken ? "Crusader Morten" : "Holy Morten";
                case "Attacker":
                    attackerNameTaken = !attackerNameTaken;
                    return attackerNameTaken ? "Munke Morten" : "Martin";
                default:
                    return ToString();
            }

        }

        public override string ToString() => "SuperMorten";

        #endregion
    }
}