using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "AirCombatState", menuName = "StateMachine/State/Player/New AirCombatState")]
    public class Player_AirCombatState : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Combat;
        [SerializeField] public bool isAirCombat;
        [SerializeField] private float playbackSpeed = 1.3f;
        public override void OnEnter()
        {
            _PlayableDirector.time = SetTime;
            SetTime = 0;
            isAirCombat = true;
            _PlayableDirector.Play(Combat);
            _PlayableDirector.extrapolationMode = isLoop;

        }

        public override void OnExit()
        {
            isAirCombat = false;
            _StateMachineSystem.currentTarget = null;

        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _StateMachineSystem.SeekTheEnemy();
            _PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(playbackSpeed);
            isAirCombat = true;
        }


        




    }
}

