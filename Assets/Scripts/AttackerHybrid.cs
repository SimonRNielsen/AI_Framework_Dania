using UnityEngine;
using AIGame.Core;

    //Bevæger sig kun i højre eller venstre hjørne

namespace MortensKombat
{

    /// <summary>
    /// HolyMorten AI implementation.
    /// TODO: Describe your AI strategy here.
    /// </summary>
    public class AttackerHybrid : SuperMorten
    {
        /// <summary>
        /// Configure the agent's stats (speed, health, etc.).
        /// </summary>
        protected override void ConfigureStats()
        {

            AllocateStat(StatType.Speed, 10); //5
            AllocateStat(StatType.ProjectileRange, 5); //10
            AllocateStat(StatType.VisionRange, 1);
            AllocateStat(StatType.DodgeCooldown, 0);
            AllocateStat(StatType.ReloadSpeed, 4);

        }

        /// <summary>
        /// Called once when the agent starts.
        /// Use this for initialization.
        /// </summary>
        protected override void StartAI()
        {

            base.StartAI();

            MKSelector rootSelector = new MKSelector(blackboard);


            //Offencesequence
            MKSequence offenceSequence = new MKSequence(blackboard);
            AquireTarget aquireTarget = new AquireTarget(blackboard, this);
            MoveInRange moveInRange = new MoveInRange(blackboard, this);
            ShootThatBunny shootThatBunny = new ShootThatBunny(blackboard, this);
            offenceSequence.children.Add(aquireTarget);
            offenceSequence.children.Add(moveInRange);
            offenceSequence.children.Add(shootThatBunny);

            //Attacker Move to controlpoint selector and sequence

            MKSequence controlPointCheckSequence = new MKSequence(blackboard); //Sequence: Outside CP? YES -> move to cp
            AmOutsideCp amOutsideCp = new AmOutsideCp(blackboard, this);
            MoveToCP moveToCP = new MoveToCP(blackboard, this);
            controlPointCheckSequence.children.Add(amOutsideCp);
            controlPointCheckSequence.children.Add(moveToCP);
            
            //Contested CP
            MKSequence contestedControlPointSequence = new MKSequence(blackboard); //Sequence: Is CP Theirs? YES -> Run controlPointCheckSequence 
            IsCPContested isCPContested = new IsCPContested(blackboard, this);
            contestedControlPointSequence.children.Add(isCPContested);
            contestedControlPointSequence.children.Add(controlPointCheckSequence);

            ////Defender moving around CP sequence
            //MKSequence moveAroundCPSequence = new MKSequence(blackboard);
            ////AmIInCP amIInCP = new AmIInCP(blackboard, this);
            //AttackerMoveAround attackerMoveAround = new AttackerMoveAround(blackboard, this);
            ////moveAroundCPSequence.children.Add(amIInCP);
            //attackerMoveAround.children.Add(attackerMoveAround);

            //virker
            //AttackerMoveToCP moveAroundCPSequencattacke = new AttackerMoveToCP(blackboard, this);
            //moveAroundCPSequencattacke.children.Add(moveAroundCPSequencattacke);

            AttackerMoveAroundHybrid moveAroundCPSequencattackeHybrid = new AttackerMoveAroundHybrid(blackboard, this);
            moveAroundCPSequencattackeHybrid.children.Add(moveAroundCPSequencattackeHybrid);



            rootSelector.children.Add(offenceSequence);
            rootSelector.children.Add(contestedControlPointSequence);
            rootSelector.children.Add(moveAroundCPSequencattackeHybrid);
            //rootSelector.children.Add(attackerMoveAround);



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

        public override string ToString() => "Attacker";

    }
}