using BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody2D;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Enemy_DisapperCharge", menuName = "StateMachine/State/Enemy/New Enemy_DisapperCharge")]
public class EnemyDisapperChargeState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset DisapperCharge_Stage1;
    [SerializeField] protected PlayableAsset DisapperCharge_Stage2;
    [SerializeField] protected float rangeDistance = 8f;

    public AudioClip DisapperAudio => Clip;
    [HideInInspector] public string DisapperName => String;

    //timeline播放完毕时调用的方法
    public void OnTimelineFinished(PlayableDirector director)
    {
        Debug.Log(isFirstStage);
        if (!isFirstStage)
        {
            _StateMachineSystem.fog.SetActive(false);
        }
        _PlayableDirector.Stop();
        _StateMachineSystem.BackLastState("Idle");
    }

    public override void OnEnter()
    {
        FacePlayer();
        switch (DisapperName)
        {
            case "DisapperCharge_Stage1":
                _PlayableDirector.Play(DisapperCharge_Stage1);
                _PlayableDirector.extrapolationMode = isLoop;
                break;
            case "DisapperCharge_Stage2":
                _PlayableDirector.Play(DisapperCharge_Stage2);
                _PlayableDirector.extrapolationMode = isLoop;
                break;
            default:
                Debug.Log("没找到闪现动画");
                break;

        }
        if (_StateMachineSystem == null)
        {
            InitState(_StateMachineSystem);
        }
        //_StateMachineSystem.AudioSourcesDictionary["Masa_FX"].clip = DisapperAudio;
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
        Debug.Log("敌人消失");
        _StateMachineSystem.SeekThePlayer();
    }

    public override Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            {"DisapperCharge_Stage1", DisapperCharge_Stage1},
            {"DisapperCharge_Stage2", DisapperCharge_Stage2},
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
        DisapperCharge_Stage1 = (PlayableAsset)data["DisapperCharge_Stage1"];
        DisapperCharge_Stage2 = (PlayableAsset)data["DisapperCharge_Stage2"];
    }
}
