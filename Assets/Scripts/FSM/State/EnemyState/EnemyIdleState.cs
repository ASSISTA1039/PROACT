using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Enemy_Idle", menuName = "StateMachine/State/Enemy/New Enemy_Idle")]
public class EnemyIdleState : EnemyStateBase
{
    [SerializeField] protected PlayableAsset Idle;
    [SerializeField] protected float rangeDistance = 5f;
    
    private bool isWaiting = false;  // 标记是否正在等待
    private int DisapperCharge_Stage2_time = 0;

    public override void OnEnter()
    {
        FacePlayer();

        _PlayableDirector.Play(Idle);
        _PlayableDirector.extrapolationMode = isLoop;
        if (_StateMachineSystem == null)
        {
            InitState(_StateMachineSystem);
        }
        // 启动延迟行为判断的协程
        _StateMachineSystem.StartCoroutine(WaitBeforeDecision());
    }

    private IEnumerator WaitBeforeDecision()
    {
        isWaiting = true; // 标记为等待状态
        yield return new WaitForSeconds(3.5f); // 等待 3.5 秒
        isWaiting = false; // 结束等待
    }

    public override void OnUpdate()
    {
        //如果BOSS在被持续挨打，如果受伤次数大于10，如果韧度条恢复，执行闪现A——一阶段，然后将受伤次数归0
        if (_StateMachineSystem.hitCount >= 30 && _StateMachineSystem.dataSO.health >= 0)
        {
            _StateMachineSystem.hitCount = 0;
            string DisapperAnimation = "DisapperCharge_Stage1";

            _StateMachineSystem.StatesDictionary["DisapperCharge"].String = DisapperAnimation;
            _StateMachineSystem.BackLastState("DisapperCharge");
        }

        if (isWaiting) return; // 如果正在等待，则不执行行为判断
        if (_StateMachineSystem.RefreshToughen) return; // 如果正在刷新硬直，则不执行行为判断
        Debug.Log("没发现敌人，待机");
        Debug.Log(_StateMachineSystem);
        //如果BOSS的血量第一次低于50%,释放烟雾，执行闪现A——2阶段
        if (DisapperCharge_Stage2_time <= 4 && isFirstStage && _StateMachineSystem.dataSO.health <= 0.5f * _StateMachineSystem.dataSO.maxHealth)
        {
            //time++,知道为3时变false
            _StateMachineSystem.fog.SetActive(true);
            DisapperCharge_Stage2_time += 1;
            if(DisapperCharge_Stage2_time <=3)
            {
                string DisapperAnimation = "DisapperCharge_Stage2";

                _StateMachineSystem.StatesDictionary["DisapperCharge"].String = DisapperAnimation;
                _StateMachineSystem.BackLastState("DisapperCharge");
            }
            else if(DisapperCharge_Stage2_time == 4)
            {
                _StateMachineSystem.fog.SetActive(false);
                isFirstStage = false;
            }


        }
        else
        {
            if (_StateMachineSystem.GetCurrentTarget() && _StateMachineSystem.GetCurrentTargetDistance() > rangeDistance)
            {

                string[] animations = { "RangeCombat_Circle", "RangeCombat_FlyPounce" };
                string RangeCombatAnimation = animations[Random.Range(0, animations.Length)];

                _StateMachineSystem.StatesDictionary["RangeCombat"].String = RangeCombatAnimation;
                _StateMachineSystem.BackLastState("RangeCombat");
            }
            else if (_StateMachineSystem.GetCurrentTarget() && _StateMachineSystem.GetCurrentTargetDistance() <= rangeDistance)
            {
                //根据BOSS血量判断是平A3段还是平A4段
                //BOSS也有可能执行转圈圈
                if (_StateMachineSystem.dataSO.health <= 0.5f * _StateMachineSystem.dataSO.maxHealth)
                {
                    string[] animations = { "RangeCombat_Circle", "CombatB" };
                    string CombatAnimation = animations[Random.Range(0, animations.Length)];
                    if (CombatAnimation.Equals("RangeCombat_Circle") || CombatAnimation.Equals("RangeCombat_FlyPounce"))
                    {
                        _StateMachineSystem.StatesDictionary["RangeCombat"].String = CombatAnimation;
                        _StateMachineSystem.BackLastState("RangeCombat");
                    }
                    else
                    {
                        _StateMachineSystem.StatesDictionary["Combat"].String = CombatAnimation;
                        _StateMachineSystem.BackLastState("Combat");
                    }
                }
                else
                {
                    string[] animations = { "RangeCombat_Circle", "CombatA"};
                    string CombatAnimation = animations[Random.Range(0, animations.Length)];

                    if (CombatAnimation.Equals("RangeCombat_Circle"))
                    {
                        _StateMachineSystem.StatesDictionary["RangeCombat"].String = CombatAnimation;
                        _StateMachineSystem.BackLastState("RangeCombat");
                    }
                    else
                    {
                        _StateMachineSystem.StatesDictionary["Combat"].String = CombatAnimation;
                        _StateMachineSystem.BackLastState("Combat");
                    }
                }
            }
        }
    }

    public override Dictionary<string, object> Copy()
    {
        var data = new Dictionary<string, object>
        {
            {"Idle", Idle},
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
        Idle = (PlayableAsset)data["Idle"];
    }
}
