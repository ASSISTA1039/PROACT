using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Enemy_RangeCombat", menuName = "StateMachine/State/Enemy/New Enemy_RangeCombat")]
public class EnemyRangeCombatState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset RangeCombat_Circle;
    [SerializeField] protected PlayableAsset RangeCombat_FlyPounce;

    public AudioClip RangeCombatAudio => Clip;
    [HideInInspector] public string RangeCombatName => String;

    //timeline播放完毕时调用的方法
    public void OnTimelineFinished(PlayableDirector director)
    {
        _PlayableDirector.Stop();
        _StateMachineSystem.BackLastState("Idle");
    }
    public override void OnEnter()
    {
        FacePlayer();
        switch (RangeCombatName)
        {
            case "RangeCombat_Circle":
                _PlayableDirector.Play(RangeCombat_Circle);
                _PlayableDirector.extrapolationMode = isLoop;
                break;
            case "RangeCombat_FlyPounce":
                _PlayableDirector.Play(RangeCombat_FlyPounce);
                _PlayableDirector.extrapolationMode = isLoop;
                break;
            default:
                Debug.Log("没找到远程动画");
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
        MoveTowardsPlayer(); // 每帧更新时朝向玩家移动
        _StateMachineSystem.SeekThePlayer();
    }

    // 让 BOSS 朝向玩家移动
    private void MoveTowardsPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // 获取玩家
        if (player == null) return;

        Vector3 direction = (player.transform.position - _BOSS.position).normalized; // 计算方向
        float distance = Vector3.Distance(_BOSS.position, player.transform.position); // 计算距离

        float minDistance = 3.5f; // 设置最小安全距离，防止 BOSS 贴脸
        float moveSpeed = 4f;   // 设定移动速度

        if (distance > minDistance)
        {
            _CharacterController.Move(direction * moveSpeed * Time.deltaTime); // 让 BOSS 移动
        }
    }



    public override Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            {"RangeCombat_Circle", RangeCombat_Circle},
            {"RangeCombat_FlyPounce", RangeCombat_FlyPounce},
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
        RangeCombat_Circle = (PlayableAsset)data["RangeCombat_Circle"];
        RangeCombat_FlyPounce = (PlayableAsset)data["RangeCombat_FlyPounce"];

    }

}
