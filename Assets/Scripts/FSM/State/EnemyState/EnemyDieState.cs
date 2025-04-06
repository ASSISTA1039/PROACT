using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Enemy_Dle", menuName = "StateMachine/State/Enemy/New Enemy_Dle")]
public class EnemyDieState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset Die;

    //timeline播放完毕时调用的方法
    public void OnTimelineFinished(PlayableDirector director)
    {
        _CharacterController.enabled = true;
        //GameObjectPoolSystem.Instance.RecyleEnemy(_Player.gameObject);
        _StateMachineSystem.BackLastState("Idle");
        _StateMachineSystem.dataSO.health = _StateMachineSystem.dataSO.maxHealth;

    }

    public override void OnEnter()
    {
        _PlayableDirector.Play(Die);
        _PlayableDirector.extrapolationMode = isLoop;
        _CharacterController.enabled = false;
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

    public override Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            {"Die", Die},
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
        Die = (PlayableAsset)data["Die"];

    }


}
