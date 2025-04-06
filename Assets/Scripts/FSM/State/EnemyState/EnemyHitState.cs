using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Enemy_Idle", menuName = "StateMachine/State/Enemy/New Enemy_Hit")]
public class EnemyHitState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset Hit_D_Up;
    [SerializeField] protected PlayableAsset Hit_Right_Left;
    [SerializeField] protected PlayableAsset Hit_Left_Right;
    [SerializeField] protected PlayableAsset Hit_Inplace;


    public AudioClip HitAudio => Clip;
    [HideInInspector] public string HitName => String;

    //timeline播放完毕时调用的方法
    public void OnTimelineFinished(PlayableDirector director)
    {
        _PlayableDirector.Stop();
        _StateMachineSystem.已经倒地 = true;
        _StateMachineSystem.BackLastState("KnockDown");
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
        //_StateMachineSystem.AudioSourcesDictionary["Masa_FX"].clip = HitAudio;
        //_StateMachineSystem.AudioSourcesDictionary["Masa_FX"].Play();

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


    public override Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            {"Hit_D_Up", Hit_D_Up},
            {"Hit_Right_Left", Hit_Right_Left},
            {"Hit_Left_Right", Hit_Left_Right},
            {"Hit_Inplace", Hit_Inplace},
        };
        foreach (var kvp in base.Copy())
        {
            data[kvp.Key] = kvp.Value;
        }

        return data;
    }
    public override void Paste(Dictionary<string, object> data)
    {
        base.Paste(data);
        Hit_D_Up = (PlayableAsset)data["Hit_D_Up"];
        Hit_Right_Left = (PlayableAsset)data["Hit_Right_Left"];
        Hit_Left_Right = (PlayableAsset)data["Hit_Left_Right"];
        Hit_Inplace = (PlayableAsset)data["Hit_Inplace"];

    }

}
