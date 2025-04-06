using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "EvadeState", menuName = "StateMachine/State/Player/New EvadeState")]
    public class Player_EvadeState : StateBaseSO
    {
        [SerializeField] protected PlayableAsset EvadeFront;
        [SerializeField] protected PlayableAsset EvadeBack;
        [SerializeField] protected PlayableAsset EvadeCombat;
        [SerializeField] protected PlayableAsset EvadeAir;

        [SerializeField] public float groundCheckDistance = 0.1f;
        [SerializeField] public LayerMask groundLayer;

        private Vector3 movementDirection;
        private bool CanEvadeCombat;

        public Player_MoveState MoveState;

        //玩家攻击力暂时
        public float tempattack;

        //timeline播放完毕时调用的方法
        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            _StateMachineSystem.BackLastState("Idle");
            
        }

        public override void OnEnter()
        {
            if (CheckGrounded())
            {
                if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
                {
                    _PlayableDirector.Play(EvadeFront);
                    _PlayableDirector.extrapolationMode = isLoop;
                    MoveState.isRun = true;
                }
                else
                {
                    _PlayableDirector.Play(EvadeBack);
                    _PlayableDirector.extrapolationMode = isLoop;
                }
            }
            else
            {
                _PlayableDirector.Play(EvadeAir);
                //_PlayableDirector.extrapolationMode = isLoop;
            }
            if (_PlayableDirector != null)
            {
                //进入状态时注册事件
                _PlayableDirector.stopped += OnTimelineFinished;
            }
            CanEvadeCombat = true;


        }

        public override void OnExit()
        {
            //MoveState.isRun = false;
            if (_PlayableDirector != null)
            {
                //退出状态时注销事件
                _PlayableDirector.stopped -= OnTimelineFinished;
            }
        }

        public override void OnUpdate()
        {
            _StateMachineSystem.SeekTheEnemy();
            //_StateMachineSystem.BackLastState(CombatState);

            if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon && _PlayableDirector.playableAsset == EvadeFront)
            {
                //角色在地面，且输入不为零，且可以移动，则赋予移动速度
                movementDirection.z = CharacterInputSystem.Instance.playerMovement.x;
                movementDirection.x = CharacterInputSystem.Instance.playerMovement.y;
                //记录摄像机的旋转角度
                float angleOfRotaion = Camera.main.transform.rotation.eulerAngles.y;
                
                _Player.rotation = Quaternion.Slerp(_Player.rotation, Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0), Time.deltaTime * 8f);
                //_Player.rotation = Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0);

            }
            if(CheckGrounded())
            {
                if (CharacterInputSystem.Instance.playerLAtk && CanEvadeCombat)
                {
                    _PlayableDirector.Play(EvadeCombat);
                    _PlayableDirector.extrapolationMode = isLoop;
                    CanEvadeCombat = false;
                }
            }

        }

        private bool CheckGrounded()
        {
            return Physics.Raycast(_Player.transform.position, Vector3.down, groundCheckDistance, groundLayer);
        }
    }
}
