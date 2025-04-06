using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "Hit_State", menuName = "StateMachine/State/Player/New Hit_State")]
    public class Player_HitState : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Hit;

        //timeline播放完毕时调用的方法
        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();

        }

        public override void OnEnter()
        {
            _PlayableDirector.Play(Hit);

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
        }
    }
}
