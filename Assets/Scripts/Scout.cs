using UnityEngine;
using AIGame.Core;

namespace MortensKombat
{
    /// <summary>
    /// UnderCoverMorten AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class Scout : SuperMorten
    {

        /// <summary>
        /// Configure the agent's stats (speed, health, etc.).
        /// </summary>
        protected override void ConfigureStats()
        {

            AllocateStat(StatType.Speed, 10);
            AllocateStat(StatType.ProjectileRange, 0);
            AllocateStat(StatType.VisionRange, 10);
            AllocateStat(StatType.DodgeCooldown, 0);
            AllocateStat(StatType.ReloadSpeed, 0);

            SpawnPosition = transform.position;
        }

        /// <summary>
        /// Called once when the agent starts.
        /// Use this for initialization.
        /// </summary>
        protected override void StartAI()
        {

            base.StartAI();

            //Scouting
            MKSequence scouting = new MKSequence(blackboard);
            MoveToScouting moveToScouting = new MoveToScouting(blackboard, this);
            scouting.children.Add(moveToScouting);

            //Fleeing
            MKSequence fleeing = new MKSequence(blackboard);
            EnemyInrange enemyInrange = new EnemyInrange(blackboard, this);
            Flee flee = new Flee(blackboard, this);
            fleeing.children.Add(enemyInrange);
            fleeing.children.Add(flee);

            //root selector
            MKSelector rootSelector = new MKSelector(blackboard);
            rootSelector.children.Add(fleeing);
            rootSelector.children.Add(scouting);

            behaviourTree = new MKTree(rootSelector, blackboard);
        }

        /// <summary>
        /// Called every frame to make decisions.
        /// Implement your AI logic here.
        /// </summary>
        protected override void ExecuteAI()
        {

            base.ExecuteAI();

        }


        public override string ToString() => "Scout";

    }
}