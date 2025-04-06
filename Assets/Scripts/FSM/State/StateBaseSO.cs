using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace Assista.FSM
{
    public class StateBaseSO : ScriptableObject
    {
        protected AnimancerComponent _Animancer;
        protected Transform _Player;
        public float _TargetSpeed;
        protected PlayableDirector _PlayableDirector;
        protected CharacterController _CharacterController;
        protected StateMachineBase _StateMachineSystem;
        [SerializeField] protected DirectorWrapMode isLoop;


        //记录角色的当前血量
        public float currentHealth;

        //虚属性
        public virtual string String { get; set; }
        public virtual AudioClip Clip { get; set; }
        public virtual bool isDelay_E { get; set; }
        public virtual bool isDelay_F { get; set; }
        public virtual float SetTime { get; set; }
        protected Vector3 movementDirection;
        //初始化“状态机”
        public void InitState(StateMachineBase stateMachineSystem)
        {
            _Animancer = stateMachineSystem.GetComponent<AnimancerComponent>();
            _PlayableDirector = stateMachineSystem.GetComponent<PlayableDirector>();
            _CharacterController = stateMachineSystem.GetComponent<CharacterController>();
            _Player = stateMachineSystem.transform.GetComponent<Transform>();
            _StateMachineSystem = stateMachineSystem;
            currentHealth = _StateMachineSystem.GetComponent<PlayerStateMachine>().health;
        }
        
        public float GetTargetSpeed()
        {
            return _TargetSpeed;
        }
        //虚方法，可以在派生类中重写
        public virtual void OnEnter() {
            AddInputActionCallBacks();
            currentHealth = _StateMachineSystem.GetComponent<PlayerStateMachine>().health;
        }
        //抽象方法，必须在派生类中必须提供该方法的具体实现
        public virtual void OnUpdate() 
        {
            if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
            {
                //角色在地面，且输入不为零，且可以移动，则赋予移动速度
                movementDirection.z = CharacterInputSystem.Instance.playerMovement.x;
                movementDirection.x = CharacterInputSystem.Instance.playerMovement.y;
                //记录摄像机的旋转角度
                float angleOfRotaion = Camera.main.transform.rotation.eulerAngles.y;

                _Player.rotation = Quaternion.Slerp(_Player.rotation, Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0), Time.deltaTime * 8f);
                //_Player.rotation = Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0);

            }
        }
        public virtual void OnExit() {
            RemoveInputActionCallBacks();
        }

        //无参数构造函数
        //public StateBaseSO() { }

        #region 输入回调
        protected void AddInputActionCallBacks()
        {
            //角色walk委托  
            if (CharacterInputSystem.Instance == null || CharacterInputSystem.Instance._playerControls == null)
            {
                //Debug.LogError("CharacterInputSystem or PlayerControls is not initialized!");
                return;
            }

            // 确认 Movement 不为 null 后注册回调
            var playerInput = CharacterInputSystem.Instance._playerControls.PlayerInput;
            if (playerInput.Movement != null)
            {
                playerInput.Movement.canceled += OnMovementCanceled;
                playerInput.Movement.performed += OnMovementPerformed;
            }
            else
            {
                Debug.LogError("PlayerInput.Movement is not initialized!");
            }
        }



        protected void RemoveInputActionCallBacks()
        {
            CharacterInputSystem.Instance._playerControls.PlayerInput.Movement.canceled -= OnMovementCanceled;
            CharacterInputSystem.Instance._playerControls.PlayerInput.Movement.performed -= OnMovementPerformed;
            //CharacterInputSystem.MainInstance.inputActions.Player.CameraLook.started -= OnMouseMovementStarted;
        }
        #endregion

        #region 相机的水平居中
        public void UpdateCameraRecenteringState(Vector2 movementInput)
        {
            if (movementInput == Vector2.zero) { return; }
            //如果玩家按住W，也取消水平居中
            if (movementInput == Vector2.up)
            {
                DisableCameraRecentering();
                return;
            }
            //根据相机的垂直角来设置居中的速度
            //根据玩家的输入的按键来设置是否居中相机
            //得到相机的垂直角度（上下角）
            float cameraVerticalAngle = Camera.main.transform.localEulerAngles.x;
            //欧拉角返回的都是一个正数-90=>270，所以减去
            if (cameraVerticalAngle > 270f)
            {
                cameraVerticalAngle -= 360f;
            }
            cameraVerticalAngle = Mathf.Abs(cameraVerticalAngle);

            if (movementInput == Vector2.down)
            {
                SetCameraRecentering(cameraVerticalAngle, _StateMachineSystem.BackWardsCameraRecenteringData);
                return;
            }
            //执行到这里就是带有水平方向的输入了：
            SetCameraRecentering(cameraVerticalAngle, _StateMachineSystem.SidewaysCameraRecenteringData);
        }

        protected void SetCameraRecentering(float cameraVerticalAngle, List<PlayerCameraRecenteringData> playerCameraRecenteringDates)
        {

            foreach (PlayerCameraRecenteringData recenteringData in playerCameraRecenteringDates)
            {
                if (!recenteringData.IsWithInAngle(cameraVerticalAngle))
                {
                    //直接退出这个元素，进行下一个元素
                    continue;
                }
                //如果在范围内：
                EnableCameraRecentering(recenteringData.waitingTime, recenteringData.recenteringTime);
                return;
            }

            //如果循环完了没有匹配的范围就关闭水平居中
            DisableCameraRecentering();
        }

        public void EnableCameraRecentering(float waitTime = -1f, float recenteringTime = -1)
        {
            _StateMachineSystem.playerCameraUtility.EnableRecentering(waitTime, recenteringTime);
        }
        public void DisableCameraRecentering()
        {
            _StateMachineSystem.playerCameraUtility.DisableRecentering();
        }
        protected void OnMovementCanceled(InputAction.CallbackContext context)
        {
            //输入停止则禁用居中（水平、垂直）
            DisableCameraRecentering();
        }
        ////当有鼠标输入或者移动方向键时调用
        //private void OnMouseMovementStarted(InputAction.CallbackContext context)
        //{
        //    UpdateCameraRecenteringState(GetPlayerMovementInputDirection());
        //}

        protected void OnMovementPerformed(InputAction.CallbackContext context)
        {
            //这里直接用事件得到的最新值
            UpdateCameraRecenteringState(context.ReadValue<Vector2>());
        }

        #endregion
    }
}

