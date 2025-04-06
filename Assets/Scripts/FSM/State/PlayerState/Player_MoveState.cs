using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "MoveState", menuName = "StateMachine/State/Player/New MoveState")]
    public class Player_MoveState : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Move;
        [SerializeField] protected PlayableAsset Move_Clone;
        [SerializeField] protected PlayableAsset Run;
        [SerializeField] protected PlayableAsset Run_Clone;
  
        [SerializeField] protected PlayableAsset TurnBack;
        [SerializeField] private float playbackSpeed = 1.5f;
        //public StateBaseSO RunEndState;
        //public StateBaseSO EvadeState;

        private bool door = true;
        private bool isTurnBack;
        private bool isTurnBackJustNow = false;
        [HideInInspector] public  bool isRun = false;
        private float angle;
        private float playerMovementInputTime = 0.05f;

        Quaternion quaternion;

        private Vector3 movementDirection;

        public float targetAngle;
        public float rotationTime;

        //timeline播放完毕时调用的方法
        public void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            door = !door;
            isTurnBack = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (_PlayableDirector != null)
            {
                //进入状态时注册事件
                _PlayableDirector.stopped += OnTimelineFinished;
            }
            
        }

        public override void OnExit()
        {
            base.OnExit();
            isRun = false;
            if(_PlayableDirector != null)
            {
                //退出状态时注销事件
                _PlayableDirector.stopped -= OnTimelineFinished;
            }
        }

        public override void OnUpdate()
        {
            if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
            {
                if (isRun)
                {
                    _TargetSpeed = 6f;
                    if (!isTurnBack) { CharacterRotation(CharacterInputSystem.Instance.playerMovement); }
                    PlayerTurnBack();
                    AlternatePlayPlayableAsset(Run, Run_Clone);
                    if (_PlayableDirector.playableGraph.IsValid())
                        _PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0.8f * playbackSpeed);
                }
                else
                {
                    _TargetSpeed = 4f;
                    AlternatePlayPlayableAsset(Move, Move_Clone);
                    if(_PlayableDirector.playableGraph.IsValid())
                        _PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(playbackSpeed);
                }
                //控制角色的公转
                //CharacterRotation(CharacterInputSystem.Instance.playerMovement);
                //点按左shift或者鼠标右键
                if (CharacterInputSystem.Instance.playerSlide || CharacterInputSystem.Instance.playerDefen)
                {
                    _StateMachineSystem.BackLastState("Evade");
                }

                if (!isTurnBack){ CharacterRotation(CharacterInputSystem.Instance.playerMovement); }

                playerMovementInputTime = 0.05f;

            }
            else
            {
                playerMovementInputTime -= Time.deltaTime;
                if (playerMovementInputTime < 0)
                {
                    _StateMachineSystem.BackLastState("RunEnd");
                }
                
            }
            if (CharacterInputSystem.Instance.playerJump)
            {
                _StateMachineSystem.BackLastState("Jump");
            }
            if (CharacterInputSystem.Instance.playerLAtk)
            {
                _StateMachineSystem.BackLastState("Combat");
            }
            if (CharacterInputSystem.Instance.Combat_E)
            {
                _StateMachineSystem.BackLastState("Combat_E");
            }
            if (CharacterInputSystem.Instance.Combat_F)
            {
                _StateMachineSystem.BackLastState("Combat_F");
            }
            if (CharacterInputSystem.Instance.Combat_Q)
            {
                _StateMachineSystem.BackLastState("Combat_Q");
            }
            if (_StateMachineSystem.GetComponent<PlayerStateMachine>().health < currentHealth)
            {
                _StateMachineSystem.BackLastState("Hit");
            }
            currentHealth = _StateMachineSystem.GetComponent<PlayerStateMachine>().health;
        }

        float currentVelocity = 0;
        protected void CharacterRotation(Vector2 movementDirection)
        {
            // 如果没有玩家输入，不旋转
            if (movementDirection == Vector2.zero) { return; }

            // 计算目标角度，叠加相机的 Y 轴旋转
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            // 平滑过渡到目标角度
            float smoothedAngle = Mathf.SmoothDampAngle(_Player.eulerAngles.y, targetAngle, ref currentVelocity, rotationTime);

            // 应用旋转
            _Player.rotation = Quaternion.Lerp(_Player.rotation, Quaternion.Euler(0, targetAngle, 0), Time.deltaTime * 20);
        }

        private void PlayerTurnBack()
        {
            ////角色转身跑
            ////Time.timeScale = 0.3f;
            //Vector3 vector = new Vector3();
            //vector.x = CharacterInputSystem.Instance.playerMovement.x;
            //vector.z = CharacterInputSystem.Instance.playerMovement.y;
            //angle = Vector3.Angle(_Player.forward, (Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * vector).normalized);

            ////当角色forward方向与移动目标方向角度大于120时触发转身跑
            //if (angle > 120)
            //{
            //    if (_PlayableDirector.playableAsset != TurnBack)
            //    {
            //        isTurnBack = true;
            //        _PlayableDirector.Play(TurnBack);
            //        _PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(playbackSpeed);
            //        _PlayableDirector.extrapolationMode = DirectorWrapMode.None;
            //        isTurnBackJustNow = true;

            //    }

            //}

        }

        private void PlayerTurn()
        {
            ////角色在地面，且输入不为零，且可以移动，则赋予移动速度
            //movementDirection.z = CharacterInputSystem.Instance.playerMovement.x;
            //movementDirection.x = CharacterInputSystem.Instance.playerMovement.y;
            ////记录摄像机的旋转角度
            //float angleOfRotaion = Camera.main.transform.rotation.eulerAngles.y;

            //quaternion = Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0);
            //if (!isTurnBackJustNow)
            //{
            //    //_Player.rotation = Quaternion.Slerp(_Player.rotation, quaternion, Time.deltaTime * 8f);
            //}
            //else
            //{
            //    _Player.rotation = quaternion;
            //   isTurnBackJustNow = false;
            //}

        }

        private void PlayerTurn1()
        {
            // 确保输入方向不为零
            if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude < Mathf.Epsilon)
                return;

            // 1. 获取玩家输入方向（将输入坐标转为3D）
            movementDirection.z = CharacterInputSystem.Instance.playerMovement.x;
            movementDirection.x = CharacterInputSystem.Instance.playerMovement.y;

            // 2. 基于摄像机角度，将输入方向转换到世界坐标系
            float angleOfRotaion = Camera.main.transform.rotation.eulerAngles.y;
            Vector3 worldDirection = Quaternion.Euler(0, angleOfRotaion, 0) * movementDirection;

            // 3. 计算目标朝向的四元数
            quaternion = Quaternion.LookRotation(worldDirection, _Player.up);

            // 4. 平滑转向（调整旋转速度：0.3f 控制平滑效果）
            _Player.rotation = Quaternion.Slerp(_Player.rotation, quaternion, Time.deltaTime * 8f);
        }


        //交替播放两个一样的timeline
        private void AlternatePlayPlayableAsset(PlayableAsset asset1, PlayableAsset asset2)
        {
            if (_PlayableDirector.playableAsset != asset1 && door && !isTurnBack)
            {
                _PlayableDirector.Play(asset1);
                _PlayableDirector.extrapolationMode = DirectorWrapMode.None;

            }
            if (_PlayableDirector.playableAsset != asset2 && !door && !isTurnBack)
            {
                _PlayableDirector.Play(asset2);
                _PlayableDirector.extrapolationMode = DirectorWrapMode.None;

            }
        }




    }
}

