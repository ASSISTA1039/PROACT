using System.Collections;
using System.Collections.Generic;
using Assista.FSM;
using Animancer;
using UnityEngine;
using UnityEngine.Playables;

public abstract class EnemyStateBase : MonoBehaviour
{
    protected AnimancerComponent _Animancer;
    protected Transform _BOSS;

    protected PlayableDirector _PlayableDirector;
    protected CharacterController _CharacterController;
    protected EnemyStateMachine _StateMachineSystem;

    [SerializeField] protected DirectorWrapMode isLoop;
    public bool isFirstStage = true;
    //虚属性
    public virtual string String { get; set; }
    public virtual AudioClip Clip { get; set; }

    //初始化“状态机”
    public void InitState(EnemyStateMachine stateMachineSystem)
    {
        _Animancer = stateMachineSystem.GetComponent<AnimancerComponent>();
        _PlayableDirector = stateMachineSystem.GetComponent<PlayableDirector>();
        _BOSS = stateMachineSystem.transform.parent.GetComponent<Transform>();
        _CharacterController = _BOSS.GetComponent<CharacterController>();
        _StateMachineSystem = stateMachineSystem;

    }

    public virtual Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            { "isLoop", isLoop},
        };
        return data;
    }

    public virtual void Paste(Dictionary<string, object> data)
    {
        isLoop = (DirectorWrapMode)data["isLoop"];

    }

    //虚方法，可以在派生类中重写
    public virtual void OnEnter() { }
    //抽象方法，必须在派生类中必须提供该方法的具体实现
    public abstract void OnUpdate();
    public virtual void OnExit() { }

    protected void FacePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // 找到玩家
        if (player != null)
        {
            Vector3 direction = player.transform.position - _BOSS.position; // 计算朝向向量
            direction.y = 0; // 只在水平面旋转，防止 BOSS 翻转
            if (direction != Vector3.zero)
            {
                _BOSS.rotation = Quaternion.LookRotation(direction); // 让 BOSS 朝向玩家
            }
        }
    }
}
