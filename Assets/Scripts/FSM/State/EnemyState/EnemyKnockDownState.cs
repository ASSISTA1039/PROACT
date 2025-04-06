using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Enemy_Idle", menuName = "StateMachine/State/Enemy/New Enemy_Hit")]
public class EnemyKnockDownState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset Knockdown;


    //timeline播放完毕时调用的方法
    public void OnTimelineFinished(PlayableDirector director)
    {
        _PlayableDirector.Stop();
        _StateMachineSystem.BackLastState("Idle");
    }
    public override void OnEnter()
    {
        _PlayableDirector.Play(Knockdown);
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
        Debug.Log("挨打了");

    }


    public override Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            {"KnockDown", Knockdown},
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
        Knockdown = (PlayableAsset)data["KnockDown"];

    }

}
