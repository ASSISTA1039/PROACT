using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Enemy_Idle", menuName = "StateMachine/State/Enemy/New Enemy_Hit")]
public class EnemyAirHitState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset AirHit;
    [SerializeField] protected PlayableAsset AirHitDown;
    [HideInInspector] public string HitName => String;

    //timeline播放完毕时调用的方法
    public void OnTimelineFinished(PlayableDirector director)
    {
        _PlayableDirector.Stop();
        _StateMachineSystem.BackLastState("KnockDown");
    }
    public override void OnEnter()
    {
        switch (HitName)
        {
            case "AirHit":
                _PlayableDirector.Play(AirHit);
                _PlayableDirector.extrapolationMode = isLoop;
                break;
            case "AirHitDown":
                _PlayableDirector.Play(AirHitDown);
                _PlayableDirector.extrapolationMode = isLoop;
                break;


        }

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
            {"AirHit", AirHit},
            {"AirHitDown", AirHitDown},
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
        AirHit = (PlayableAsset)data["AirHit"];
        AirHitDown = (PlayableAsset)data["AirHitDown"];

    }

}
