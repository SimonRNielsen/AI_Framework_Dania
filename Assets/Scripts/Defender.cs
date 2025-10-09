using UnityEngine;
using AIGame.Core;

namespace MortensKombat
{
    /// <summary>
    /// CrusaderMorten AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class Defender : SuperMorten
    {

        #region Fields

        public bool inCP;

        #endregion
        #region Properties
        #endregion

        #region Constructor
        #endregion

        #region Methods
        /// <summary>
        /// Configure the agent's stats (speed, health, etc.).
        /// </summary>
        protected override void ConfigureStats()
        {

            AllocateStat(StatType.Speed, 9);
            AllocateStat(StatType.ProjectileRange, 0);
            AllocateStat(StatType.VisionRange, 4);
            AllocateStat(StatType.DodgeCooldown, 7);
            AllocateStat(StatType.ReloadSpeed, 0);

        }

        /// <summary>
        /// Called once when the agent starts.
        /// Use this for initialization.
        /// </summary>
        protected override void StartAI()
        {

            base.StartAI();

            //Attack sequence
            MKSequence combatSequence = new MKSequence(blackboard);
            IsEnemyInVisibleRange isEnemyInVisibleRange = new IsEnemyInVisibleRange(blackboard, this);
            ShootThatBunny shootThatBunny = new ShootThatBunny(blackboard, this);
            combatSequence.children.Add(isEnemyInVisibleRange);
            combatSequence.children.Add(shootThatBunny);

            //Defender Move to controlpoint sequence
            MKSequence controlPointCheck = new MKSequence(blackboard);
            AmOutsideCp amOutsideCp = new AmOutsideCp(blackboard, this);
            MoveToCP moveToCP = new MoveToCP(blackboard, this);
            controlPointCheck.children.Add(amOutsideCp);
            controlPointCheck.children.Add(moveToCP);

            //Defender moving around CP sequence
            MKSequence moveAroundCPSequence = new MKSequence(blackboard);
            AmIInCP amIInCP = new AmIInCP(blackboard, this);
            MoveAroundCP moveAroundCP = new MoveAroundCP(blackboard, this);
            moveAroundCPSequence.children.Add(amIInCP);
            moveAroundCPSequence.children.Add(moveAroundCP);

            MKSelector defenderSelector = new MKSelector(blackboard);
            defenderSelector.children.Add(controlPointCheck);
            defenderSelector.children.Add(moveAroundCP);

            //Root selector
            MKSelector rootSelector = new MKSelector(blackboard);
            rootSelector.children.Add(combatSequence); //Prioritises shooting. Note that it still runs. 
            rootSelector.children.Add(defenderSelector);

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

        public override string ToString() => "Defender";
        #endregion

        

    }
}