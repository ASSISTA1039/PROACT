using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "Combat_Q_State", menuName = "StateMachine/State/Player/New Combat_Q_State")]
    public class Combat_Q_State : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Combat_Q;


        //timeline�������ʱ���õķ���
        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            
        }

        public override void OnEnter()
        {

            _PlayableDirector.Play(Combat_Q);
            _StateMachineSystem.isBOSSStaticStop = true;
            _PlayableDirector.extrapolationMode = DirectorWrapMode.None;

            if (_PlayableDirector != null)
            {
                //����״̬ʱע���¼�
                _PlayableDirector.stopped += OnTimelineFinished;
            }
        }

        public override void OnExit()
        {
            _StateMachineSystem.isBOSSStaticStop = false;
            if (_PlayableDirector != null)
            {
                //�˳�״̬ʱע���¼�
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
