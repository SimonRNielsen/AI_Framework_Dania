using AIGame.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Simon.AI
{
    /// <summary>
    /// SimonBehaviourAI AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class BehaviourAITest : BaseAI
    {

        private MKTree behaviourTree;
        private MKBlackboard blackboard;

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

            blackboard = MKBlackboard.GetShared(this);

            MKSequence combatSequence = new MKSequence(blackboard);
            combatSequence.children.Add(new SetTarget(blackboard, this));
            combatSequence.children.Add(new AttackEnemy(blackboard, this));

            InvestigateLastSeenEnemy investigateLastSeenEnemy = new InvestigateLastSeenEnemy(blackboard, this);

            MoveToPoint moveToPoint = new MoveToPoint(blackboard, this, transform.position);

            MKSelector rootSelector = new MKSelector(blackboard);
            rootSelector.children.Add(combatSequence);
            rootSelector.children.Add(investigateLastSeenEnemy);
            rootSelector.children.Add(moveToPoint);

            behaviourTree = new MKTree(rootSelector, blackboard);

        }

        /// <summary>
        /// Called every frame to make decisions.
        /// Implement your AI logic here.
        /// </summary>
        protected override void ExecuteAI()
        {
            
            NodeState result = behaviourTree.Tick();

        }

    }

}