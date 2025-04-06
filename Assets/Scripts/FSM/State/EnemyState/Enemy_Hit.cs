using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEditor.Timeline;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "Enemy_Hit", menuName = "StateMachine/State/Enemy/Enemy_Hit")]
    public class Enemy_Hit : StateBaseSO
    {
        //[SerializeField] protected PlayableAsset Idle;
        [SerializeField] protected PlayableAsset Hit_D_Up;
        [SerializeField] protected PlayableAsset Hit_Right_Left;
        [SerializeField] protected PlayableAsset Hit_Left_Right;
        [SerializeField] protected PlayableAsset Hit_Inplace;

        //[SerializeField] protected StateBaseSO IdleState;

        //public ExposedReference<AudioSource> _AudioSource;
        //public AudioSource _AudioSource1;



        public AudioClip HitAudio => Clip;

        [HideInInspector] public string HitName => String;

        //timeline播放完毕时调用的方法
        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            _StateMachineSystem.BackLastState("Idle");
        }

        public override void OnEnter()
        {
            switch (HitName)
            {
                case "Hit_D_Up":
                    _PlayableDirector.Play(Hit_D_Up);
                    _PlayableDirector.extrapolationMode = isLoop;
                    break;
                case "Hit_Right_Left":
                    _PlayableDirector.Play(Hit_Right_Left);
                    _PlayableDirector.extrapolationMode = isLoop;
                    break;
                case "Hit_Left_Right":
                    _PlayableDirector.Play(Hit_Left_Right);
                    _PlayableDirector.extrapolationMode = isLoop;
                    break;
                case "Hit_Inplace":
                    _PlayableDirector.Play(Hit_Inplace);
                    _PlayableDirector.extrapolationMode = isLoop;
                    break;
                default:
                    Debug.Log("没找到受伤动画");
                    break;

            }
            _StateMachineSystem.AudioSourcesDictionary["Masa_FX"].clip = HitAudio;
            _StateMachineSystem.AudioSourcesDictionary["Masa_FX"].Play();

            //AudioSourcesDictionary["Masa_FX"].clip = HitAudio;
            //AudioSourcesDictionary["Masa_FX"].Play();

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
            //Debug.Log("挨打了");


        }


    }
}

