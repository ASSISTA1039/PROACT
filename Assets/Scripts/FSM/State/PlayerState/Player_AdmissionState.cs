using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "AdmissionState", menuName = "StateMachine/State/Player/New AdmissionState")]
    public class Player_AdmissionState : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Admission;

        //[SerializeField, Header("敌人检测")] private float detectionRang;
        //[SerializeField] protected LayerMask enemyLayer;

        //缓存检测到的敌人目标
        //private Collider[] detectionedTarget = new Collider[1];


        //timeline播放完毕时调用的方法
        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            _StateMachineSystem.BackLastState("Idle");

        }
        public override void OnEnter()
        {
            _PlayableDirector.Play(Admission);
            _PlayableDirector.extrapolationMode = isLoop;

            if (_PlayableDirector != null)
            {
                //进入状态时注册事件
                _PlayableDirector.stopped += OnTimelineFinished;
            }
        }

        public override void OnExit()
        {
            _StateMachineSystem.currentTarget = null;

            if (_PlayableDirector != null)
            {
                //退出状态时注销事件
                _PlayableDirector.stopped -= OnTimelineFinished;
            }

        }

        public override void OnUpdate()
        {
            /*//点按左shift或者鼠标右键
            if (_InputSystem.playerSlide || _InputSystem.playerDefen)
            {
                _StateMachineSystem.BackLastState("Evade");
            }
            if (_InputSystem.playerMovement.sqrMagnitude > Mathf.Epsilon)
            {
                _StateMachineSystem.BackLastState("Move");
            }
            if (_InputSystem.playerLAtk)
            {
                _StateMachineSystem.BackLastState("Combat");
            }
            if (_InputSystem.Combat_E)
            {
                _StateMachineSystem.BackLastState("Combat_E");
            }
            if (_InputSystem.Combat_Q)
            {
                _StateMachineSystem.BackLastState("Combat_Q");
            }*/


        }



    }
}

