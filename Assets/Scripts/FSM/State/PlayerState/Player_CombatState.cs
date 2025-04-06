using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "CombatState", menuName = "StateMachine/State/Player/New CombatState")]
    public class Player_CombatState : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Combat;
        [SerializeField] protected PlayableAsset LongPress_Combat;
        [SerializeField] private float playbackSpeed = 1.5f;

        public override void OnEnter()
        {
            _PlayableDirector.time = SetTime;
            SetTime = 0;
            if(CharacterInputSystem.Instance.playerLAtkLong)
            {
                if (_StateMachineSystem.energy >= 10f)
                {
                    _StateMachineSystem.energy -= 10f;
                    _StateMachineSystem.enegySlider.fillAmount = _StateMachineSystem.energy / _StateMachineSystem.MaxEnergy;
                }
                else
                    OnExit();
                _PlayableDirector.Play(LongPress_Combat);
            }
            else if(CharacterInputSystem.Instance.playerLAtk)
            {
                _PlayableDirector.Play(Combat);
            }
            _PlayableDirector.extrapolationMode = isLoop;
            //SetPlaybackSpeed(_PlayableDirector, playbackSpeed);

        }

        public override void OnExit()
        {
            _StateMachineSystem.currentTarget = null;

        }

        public override void OnUpdate()
        {
            //base.OnUpdate();
            _StateMachineSystem.SeekTheEnemy();
            //_PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(playbackSpeed);

        }

        void SetPlaybackSpeed(PlayableDirector playableDirector, float playbackSpeed)
        {
            var graph = playableDirector.playableGraph;
            int rootPlayableCount = graph.GetRootPlayableCount();

            for (int i = 0; i < rootPlayableCount; i++)
            {
                var rootPlayable = graph.GetRootPlayable(i);
                if (rootPlayable.IsValid())
                {
                    rootPlayable.SetSpeed(playbackSpeed);
                }
            }
        }
    }
}

