using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "Combat_F_State", menuName = "StateMachine/State/Player/New Combat_F_State")]
    public class Combat_F_State : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Combat_F_Upper;
        [SerializeField] protected PlayableAsset Combat_F_StrongUpper;
        [SerializeField] protected PlayableAsset Combat_F_Down;

        [SerializeField] public Player_JumpState player_JumpState;

        [SerializeField] public float groundCheckDistance = 0.1f;
        [SerializeField] public LayerMask groundLayer;
        //public new bool isDelay_E => isDelay_E;

        //timeline播放完毕时调用的方法
        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            
        }

        public override void OnEnter()
        {
            //有无前摇
            //Debug.Log(CharacterInputSystem.Instance.Combat_F_Long);
            if (CharacterInputSystem.Instance.Combat_F_Long)
            {
                _PlayableDirector.Play(Combat_F_StrongUpper);
            }
            else if (CharacterInputSystem.Instance.Combat_F)
            {
                if(CheckGrounded())
                {
                    _PlayableDirector.Play(Combat_F_Upper);
                    isDelay_F = true;
                }
                else
                {
                    player_JumpState.isFalling = false;
                    player_JumpState.groundCheckDistance = 0.1f;
                    _PlayableDirector.Play(Combat_F_Down);
                }
            }
            _PlayableDirector.extrapolationMode = DirectorWrapMode.None;


            if (_PlayableDirector != null)
            {
                //进入状态时注册事件
                _PlayableDirector.stopped += OnTimelineFinished;
            }
        }

        public override void OnExit()
        {
            if (_PlayableDirector != null)
            {
                //退出状态时注销事件
                _PlayableDirector.stopped -= OnTimelineFinished;
            }
        }

        public override void OnUpdate()
        {
            _StateMachineSystem.SeekTheEnemy();

        }

        private bool CheckGrounded()
        {
            return Physics.Raycast(_Player.transform.position, Vector3.down, groundCheckDistance, groundLayer);
        }
    }
}
