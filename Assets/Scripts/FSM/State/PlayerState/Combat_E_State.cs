using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "Combat_E_State", menuName = "StateMachine/State/Player/New Combat_E_State")]
    public class Combat_E_State : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Combat_E;

        //public new bool isDelay_E => isDelay_E;

        //timeline播放完毕时调用的方法
        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            
        }

        public override void OnEnter()
        {
            _PlayableDirector.Play(Combat_E);

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
            _StateMachineSystem.iswudi = true;
        }
    }
}
