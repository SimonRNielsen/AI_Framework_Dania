using AIGame.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

namespace Simon.AI
{

    public enum States
    {

        None,
        Attack,
        Roam,
        Dodge,
        GetFlag,
        CaptureFlag,
        SaveFlag,
        GetPowerUp,
        CheckDirection

    }

    /// <summary>
    /// Simon_AI_Test AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class Simon_AI : BaseAI
    {

        private const float ARRIVAL_THRESHOLD = 0.5f;
        private const float incomingDot = 0.9f;
        private const float allowNewPowerUp = 0.45f;
        private States currentState = States.None;
        private Vector3 pointOfInterest;
        private IState<Simon_AI> activeState;
        private static Dictionary<States, IState<Simon_AI>> states = new Dictionary<States, IState<Simon_AI>>();
        private List<States> dontFallbackTo = new List<States>() { States.None, States.Attack, States.CheckDirection, States.Dodge, States.GetPowerUp };
        private List<Ball> checkedBalls = new List<Ball>();

        public Func<int, bool> TryConsumePowerupAction;
        public static Action<int, int> ClaimedTargetPowerUp;
        public Vector3 CurrentDestination;
        public PerceivedPowerUp TargetPowerUp;

        public Vector3 PointOfInterest { get => pointOfInterest; set => pointOfInterest = value; }

        public States PreviousState { get; set; } = States.Roam;

        public States StateBeforeFlagCapture { get; set; } = States.Roam;

        public float CheckedFor { get; set; } = 0f;

        public float TimeSinceLastPowerUp { get; set; } = 1f;

        public bool IsAttacking { get; set; } = false;

        public bool IsGettingPowerUp { get; set; } = false;

        public bool FoundFlag { get; set; } = false;

        public bool HasFlag { get; set; } = false;

        public bool OwnFlagCarried { get; set; }

        public bool EnemyFlagCarried { get; set; }

        public States CurrentState
        {
            get => currentState;
            set
            {

                if (value == currentState) return;

                if (currentState == States.CaptureFlag)
                    switch (value)
                    {
                        case States.CheckDirection:
                            if (pointOfInterest != Vector3.zero)
                                FaceTarget(pointOfInterest);
                            return;
                        default:
                            break;
                    }

                if (!dontFallbackTo.Contains(currentState))
                    PreviousState = currentState;

                if (activeState != null)
                    activeState.Exit(this);

                currentState = value;

                if (states.TryGetValue(currentState, out IState<Simon_AI> state))
                    activeState = state;
                else
                    AddNewState(currentState);

                if (activeState != null)
                    activeState.Enter(this);

            }
        }

        /// <summary>
        /// Configure the agent's stats (speed, health, etc.).
        /// </summary>
        protected override void ConfigureStats()
        {

            AllocateStat(StatType.Speed, 3);
            AllocateStat(StatType.VisionRange, 7);
            AllocateStat(StatType.ProjectileRange, 7);
            AllocateStat(StatType.ReloadSpeed, 1);
            AllocateStat(StatType.DodgeCooldown, 2);

        }

        /// <summary>
        /// Called once when the agent starts.
        /// Use this for initialization.
        /// </summary>
        protected override void StartAI()
        {

            CaptureTheFlag.Instance.FlagPickedUp += FlagPickedUp;
            CaptureTheFlag.Instance.FlagDropped += FlagDropped;
            CaptureTheFlag.Instance.FlagReturned += FlagReturned;
            CaptureTheFlag.Instance.FlagCaptured += FlagCaptured;
            ClaimedTargetPowerUp += AbandonPowerUp;
            TryConsumePowerupAction += (id) => TryConsumePowerup(id);
            MyFlagCarrier.Pickup += () => HasFlag = true;
            MyFlagCarrier.Drop += () => HasFlag = false;
            MyFlagCarrier.Capture += () => HasFlag = false;
            EnemyEnterVision += () => CurrentState = States.Attack;
            EnemyExitVision += SetPreviousState;
            FlagEnter += SpottedFlag;
            FlagExit += LostSightOffFlag;
            PowerUpEnterVision += DetectedPowerUP;
            BallDetected += SawBall;
            Respawned += () => CurrentState = States.Roam;

            CurrentDestination = transform.position;
            TimeSinceLastPowerUp = UnityEngine.Random.Range(0f, 0.4f);

        }

        private void OnDisable()
        {

            CaptureTheFlag.Instance.FlagPickedUp -= FlagPickedUp;
            CaptureTheFlag.Instance.FlagDropped -= FlagDropped;
            CaptureTheFlag.Instance.FlagReturned -= FlagReturned;
            CaptureTheFlag.Instance.FlagCaptured -= FlagCaptured;
            ClaimedTargetPowerUp -= AbandonPowerUp;
            TryConsumePowerupAction -= (id) => TryConsumePowerup(id);
            MyFlagCarrier.Pickup -= () => HasFlag = true;
            MyFlagCarrier.Drop -= () => HasFlag = false;
            MyFlagCarrier.Capture -= () => HasFlag = false;
            EnemyEnterVision -= () => CurrentState = States.Attack;
            EnemyExitVision -= SetPreviousState;
            FlagEnter -= SpottedFlag;
            FlagExit -= LostSightOffFlag;
            PowerUpEnterVision -= DetectedPowerUP;
            BallDetected -= SawBall;
            Respawned -= () => CurrentState = States.Roam;

        }

        /// <summary>
        /// Called every frame to make decisions.
        /// Implement your AI logic here.
        /// </summary>
        protected override void ExecuteAI()
        {

            if (TimeSinceLastPowerUp <= allowNewPowerUp)
                TimeSinceLastPowerUp += Time.deltaTime;

            if (activeState != null)
                activeState.Update(this);
            else
                CurrentState = States.Roam;

            if (checkedBalls.Count > 50)
            {

                List<Ball> balls = new List<Ball>();

                for (int i = 35; i < checkedBalls.Count; i++)
                    balls.Add(checkedBalls[i]);

                checkedBalls = balls;

            }

        }

        private void SetPreviousState()
        {

            if (IsAlive)
                CurrentState = PreviousState;

        }

        private void FlagPickedUp(Team id)
        {

            if (id != MyDetectable.TeamID)
            {

                FoundFlag = false;
                EnemyFlagCarried = true;

            }
            else
                OwnFlagCarried = true;

        }

        private void SpottedFlag(Team id)
        {

            if (id != MyDetectable.TeamID && !EnemyFlagCarried)
            {

                FoundFlag = true;
                CurrentState = States.GetFlag;

            }

        }

        private void SawBall(Ball ball)
        {

            if (ball.Parent.TeamID == MyDetectable.TeamID || checkedBalls.Contains(ball))
                return;
            else
                checkedBalls.Add(ball);

            Vector3 poi = ball.Parent.transform.position;
            pointOfInterest = poi;

            if (ball.Armed && CanDodge())
            {

                Vector3 ballDirection = (ball.transform.position - pointOfInterest).normalized;

                Vector3 toDodger = (transform.position - ball.transform.position).normalized;

                float dot = Vector3.Dot(ballDirection, toDodger);

                if (dot > incomingDot)
                {

                    CurrentState = States.Dodge;
                    return;

                }

            }

            if (IsAlive && pointOfInterest != Vector3.zero)
                CurrentState = States.CheckDirection;

        }

        private void LostSightOffFlag(Team id)
        {

            if (id != MyDetectable.TeamID)
                FoundFlag = false;

        }


        private void FlagDropped(Team id)
        {

            if (id != MyDetectable.TeamID)
            {

                EnemyFlagCarried = false;
                HasFlag = false;

            }

        }

        private void FlagReturned(Team id)
        {

            if (id != MyDetectable.TeamID)
                EnemyFlagCarried = false;
            else
                OwnFlagCarried = false;

        }

        private void FlagCaptured(Team id)
        {

            HasFlag = false;
            EnemyFlagCarried = false;
            OwnFlagCarried = false;
            FoundFlag = false;

        }

        private void DetectedPowerUP()
        {

            if (IsGettingPowerUp || TimeSinceLastPowerUp < allowNewPowerUp || CurrentState == States.GetFlag || CurrentState == States.CaptureFlag) return;

            IsGettingPowerUp = true;

            CurrentState = States.GetPowerUp;

        }


        /// <summary>
        /// Checks if we have arrived at our destination
        /// </summary>
        /// <returns>true if we have arrived</returns>
        public bool HasReachedDestination()
        {

            if (!NavMeshAgent.pathPending && !NavMeshAgent.hasPath && Vector3.Distance(transform.position, CurrentDestination) <= ARRIVAL_THRESHOLD)
            {

                //Debug.Log($"Reached Destination, currentstate is {currentState}");
                return true;

            }

            return false;

        }

        private void AbandonPowerUp(int id, int agentID)
        {

            if (agentID == AgentID) return;
            if (id == TargetPowerUp.Id && CurrentState == States.GetPowerUp)
            {

                CurrentState = PreviousState;
                IsGettingPowerUp = false;

            }

        }


        private void AddNewState(States state)
        {

            IState<Simon_AI> newState = null;

            switch (state)
            {
                case States.Attack:
                    newState = new AttackState();
                    break;
                case States.Roam:
                    newState = new RoamState();
                    break;
                case States.Dodge:
                    newState = new DodgeState();
                    break;
                case States.CaptureFlag:
                    newState = new CaptureFlagState();
                    break;
                case States.SaveFlag:
                    newState = new SaveFlagState();
                    break;
                case States.GetPowerUp:
                    newState = new GetPowerUpState();
                    break;
                case States.GetFlag:
                    newState = new GetFlagState();
                    break;
                case States.CheckDirection:
                    newState = new CheckDirectionState();
                    break;
            }

            if (newState == null)
            {

                Debug.LogWarning("Missing state reference in Simon_AI -> AddNewState switch");
                return;

            }

            states.TryAdd(state, newState);
            activeState = newState;

        }

    }
}