using AIGame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

namespace Simon.AI
{
    /// <summary>
    /// Simon_AI_Test AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class Simon_AI_Test : BaseAI
    {

        /// <summary>
        /// How far away from your destination we will accept as arrived
        /// </summary>
        public const float ARRIVAL_THRESHOLD = 0.5f;

        /// <summary>
        /// The current destination this agent is walking towards
        /// </summary>
        public Vector3 CurrentDestination;


        public bool IsAttacking = false;

        public bool IsGettingPowerUp = false;

        public bool IsGettingFlag = false;

        public bool FoundFlag = false;

        public bool PickingUpFlag = false;

        public bool HasFlag = false;

        public PerceivedPowerUp TargetPowerUp;

        private const float pickupTimer = 4f;

        private States_Simon currentState = States_Simon.None;

        public States_Simon PreviousState { get; set; }

        private IState_Simon<Simon_AI_Test> activeState;

        private static Dictionary<States_Simon, IState_Simon<Simon_AI_Test>> states = new Dictionary<States_Simon, IState_Simon<Simon_AI_Test>>();

        public bool OwnFlagCarried { get; set; }

        public bool EnemyFlagCarried { get; set; }

        public States_Simon CurrentState
        {
            get => currentState;
            set
            {

                if (value == currentState) return;

                if (currentState != States_Simon.None)
                    PreviousState = currentState;

                if (activeState != null)
                    activeState.Exit(this);

                if (states.TryGetValue(value, out IState_Simon<Simon_AI_Test> state))
                    activeState = state;
                else
                    AddNewState(value);

                activeState.Enter(this);

                currentState = value;

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
            MyFlagCarrier.Pickup += GotFlag;
            MyFlagCarrier.Drop += () => HasFlag = false;
            MyFlagCarrier.Capture += () => HasFlag = false;
            EnemyEnterVision += () => CurrentState = States_Simon.AttackState;
            EnemyExitVision += () => CurrentState = PreviousState;
            FlagEnter += SpottedFlag;
            FlagExit += LostSightOffFlag;
            PowerUpEnterVision += DetectedPowerUP;
            BallDetected += SawBall;

        }

        private void OnDisable()
        {

            CaptureTheFlag.Instance.FlagPickedUp -= FlagPickedUp;
            CaptureTheFlag.Instance.FlagDropped -= FlagDropped;
            CaptureTheFlag.Instance.FlagReturned -= FlagReturned;
            CaptureTheFlag.Instance.FlagCaptured -= FlagCaptured;
            MyFlagCarrier.Pickup -= GotFlag;
            MyFlagCarrier.Drop -= () => HasFlag = false;
            MyFlagCarrier.Capture -= () => HasFlag = false;
            EnemyEnterVision -= () => CurrentState = States_Simon.AttackState;
            EnemyExitVision -= () => CurrentState = PreviousState;
            FlagEnter -= SpottedFlag;
            FlagExit -= LostSightOffFlag;
            PowerUpEnterVision -= DetectedPowerUP;
            BallDetected -= SawBall;

        }

        /// <summary>
        /// Called every frame to make decisions.
        /// Implement your AI logic here.
        /// </summary>
        protected override void ExecuteAI()
        {

            if (activeState != null)
                activeState.Update(this);
            else
                CurrentState = States_Simon.RoamState;

            //if (PickingUpFlag)
            //    NavMeshAgent.isStopped = true;

            //if (!IsAttacking)
            //    isGettingFlag = GetFlag();

            //if (&& !isGettingFlag)
            //    isGettingPowerUp = GetPowerUps();

            //if (!IsGettingFlag && !IsGettingPowerUp)
            //    DoThisInstead();

        }
        private void FlagPickedUp(Team team)
        {

            if (team != MyDetectable.TeamID)
            {

            }

        }

        private void SpottedFlag(Team id)
        {

        }

        private void SawBall(Ball ball)
        {

        }

        private void LostSightOffFlag(Team id)
        {

        }


        private void FlagDropped(Team id)
        {

        }

        private void FlagReturned(Team id)
        {

        }

        private void FlagCaptured(Team id)
        {

        }

        private void DetectedPowerUP()
        {

        }

        private bool GetPowerUps()
        {

            var powerUps = GetVisiblePowerUpsSnapshot();
            float maxDistance = 1000;
            Vector3 newDestination = Vector3.zero;

            if (powerUps.Count > 0)
                foreach (var power in powerUps)
                {
                    float distance = Vector3.Distance(gameObject.transform.position, power.Position);
                    if (distance < maxDistance)
                    {
                        newDestination = power.Position;
                        maxDistance = distance;
                        TargetPowerUp = power;
                    }
                }

            if (newDestination != Vector3.zero)
            {
                NavMeshAgent.isStopped = false;
                CurrentDestination = newDestination;
                MoveTo(newDestination);
                return true;
            }

            return false;

        }


        private bool GetFlag()
        {

            var detectFlag = GetVisibleFlagsSnapShot();

            if (detectFlag.Count > 0)
            {
                foreach (var flag in detectFlag)
                {
                    if (flag.Team == MyDetectable.TeamID)
                    {
                        Debug.Log("Own flag detected");
                        continue;
                    }
                    else
                    {
                        Debug.Log("Enemy flag detected");
                        CurrentDestination = flag.Position;
                        MoveTo(CurrentDestination);
                        FoundFlag = true;
                        return true;
                    }
                }
            }

            return false;

        }

        /// <summary>
        /// Checks if we have arrived at our destination
        /// </summary>
        /// <returns>true if we have arrived</returns>
        public bool HasReachedDestination()
        {

            if (NavMeshAgent.remainingDistance <= ARRIVAL_THRESHOLD && FoundFlag)
            {

                if (!PickingUpFlag)
                {

                    PickingUpFlag = true;
                    StartCoroutine(PickupFlag());

                }

            }
            else if (NavMeshAgent.remainingDistance <= ARRIVAL_THRESHOLD && !FoundFlag)
            {

                //TryConsumePowerup(targetPowerUp.Id);

                if (HasFlag)
                    HasFlag = false;

                return true;

            }
            else if (!NavMeshAgent.pathPending && !NavMeshAgent.hasPath && Vector3.Distance(transform.position, CurrentDestination) <= ARRIVAL_THRESHOLD)
            {

                return true;

            }

            return false;

        }


        private IEnumerator PickupFlag()
        {

            yield return new WaitForSeconds(pickupTimer);

            FoundFlag = false;
            PickingUpFlag = false;

        }


        private void GotFlag()
        {

            HasFlag = true;
            Vector3? flagPosition = CaptureTheFlag.Instance.GetOwnFlagPosition(this);
            CurrentDestination = flagPosition.Value;
            NavMeshAgent.isStopped = false;
            MoveTo(CurrentDestination);

        }




        private void AddNewState(States_Simon state)
        {

            IState_Simon<Simon_AI_Test> newState = null;

            switch (state)
            {
                case States_Simon.AttackState:
                    newState = new AttackState();
                    break;
                case States_Simon.RoamState:
                    newState = new RoamState();
                    break;
                case States_Simon.DodgeState:
                    newState = new DodgeState();
                    break;
                case States_Simon.CaptureFlagState:
                    newState = new CaptureFlagState();
                    break;
                case States_Simon.SaveFlagState:
                    newState = new SaveFlagState();
                    break;
                case States_Simon.GetPowerUpState:
                    newState = new GetPowerUpState();
                    break;
            }

            if (newState == null)
            {
                Debug.Log("Missing state reference");
                return;
            }

            states.TryAdd(state, newState);
            activeState = newState;

        }

    }
}