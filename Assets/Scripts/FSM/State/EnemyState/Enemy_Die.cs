using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "Enemy_Dle", menuName = "StateMachine/State/Enemy/Enemy_Dle")]
    public class Enemy_Dle : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Die;


        //timeline播放完毕时调用的方法
        public void OnTimelineFinished(PlayableDirector director)
        {


        }

        public override void OnEnter()
        {
            _PlayableDirector.Play(Die);
            _PlayableDirector.extrapolationMode = isLoop;

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
            Debug.Log("亖了！");


        }
    }
}