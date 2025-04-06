using System.Collections;
using System.Collections.Generic;
using Animancer;
using Assista.FSM;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public abstract class CharacterMoveMentControllerBase : MonoBehaviour
{
    public AnimancerComponent _Animancer;
    public CharacterController _Controller;
    protected PlayableDirector _PlayableDirector;
    
    
    protected PlayerControls _playerControls;
    [SerializeField, Tooltip("角色动画移动时检测障碍物的层级")] protected LayerMask whatIsObs;

    //用于存储历史deltaPosition的队列
    private Queue<Vector3> deltaPositions = new Queue<Vector3>();

    private Vector3 averageDeltaPosition = new Vector3();

    //MoveDirection(移动向量：水平、垂直)
    protected Vector3 movementDirection;
    protected Vector3 verticalDirection;

    [SerializeField, Header("角色重力")] public float characterGravity;
    public float currentGravity;
    private float tempGravity;
    //[SerializeField, Header("角色当前移动速度")] protected float characterCurrentMoveSpeed;
    public float verticalSpeed;//当前角色Y轴速度
    public Vector3 inertia = Vector3.zero;
    public bool isInAirAttack = false; // 标志变量：是否处于空中攻击状态

    //跳跃相关
    private bool isReadyJump = false;

    //boss
    public GameObject BOSS;

    protected virtual void Start()
    {
        //_Animancer = GetComponent<AnimancerComponent>();
        _Controller = GetComponent<CharacterController>();
        if(_Controller == null)
        {
            _Controller = GetComponentInParent<CharacterController>();
        }
        _PlayableDirector = GetComponent<PlayableDirector>();
        _Animancer = GetComponent<AnimancerComponent>();
        _Animancer.Playable.Speed = 2f;
        currentGravity = characterGravity;
        isInAirAttack = false;
    }

    protected virtual void Update()
    {
        GetAverageDeltaPosition();
        CaculateGravity();
        //_Controller.Move(_StateMachine.CurrentState.GetTargetSpeed() * Time.deltaTime * GetRelativeDiretion(CharacterInputSystem.Instance.playerMovement));
        //transform.LookAt(transform.position + GetRelativeDiretion(CharacterInputSystem.Instance.playerMovement));
    }


    protected void GetAverageDeltaPosition()
    {
        deltaPositions.Enqueue(_Animancer.Animator.deltaPosition);
        if (deltaPositions.Count > 10)
        {
            deltaPositions.Dequeue();
        }

        Vector3 virs = new Vector3();
        foreach (Vector3 vir in deltaPositions)
        {
            virs += vir;
        }
        averageDeltaPosition = virs / 10;

    }

    protected bool CanAnimationMotion()
    {
        Debug.DrawRay(transform.position + transform.up * 0.5f, averageDeltaPosition.normalized * 0.5f, Color.red);

        //检测前方是否有障碍物(characterAnimator.GetFloat(animationMoveID)速度防止在攻击前进时与敌人重叠)
        return Physics.Raycast(transform.position + transform.up * 0.5f, averageDeltaPosition, out var hit, 0.5f, whatIsObs);

    }

    public Vector3 GetRelativeDiretion(Vector3 input)
    {
        Quaternion rot = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        Vector3 dir = rot * Vector3.forward * input.y + rot * Vector3.right * input.x;
        return new Vector3(dir.x, 0, dir.y).normalized;
    }


    public void SetJumpVelocityForUpper()
    {
        if(verticalSpeed < 1f)
        {
            verticalSpeed = 10f;
        }
        if(BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed < 1f)
        {
            //BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = 0f;
            BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = 12f;
            
        }
    }

    //public void SetJumpVelocityForUpper_BOSS()
    //{
    //    if (verticalSpeed < 1f)
    //    {
    //        verticalSpeed = 8f;
    //    }
    //    BOSS.GetComponent<EnemyMoveController>().verticalSpeed = 8f;

    //}

    public void SetJumpVelocityForDown()
    {

        if(_PlayableDirector.playableAsset.name.Contains("Jump") && verticalSpeed >= 0.1f)
        {
            //取消角色可能在空中攻击的零重力状态
            isInAirAttack = false;
            currentGravity = characterGravity;
            //verticalSpeed = -5f;
        }
        else if (verticalSpeed > -10f)
        {
            //取消角色可能在空中攻击的零重力状态
            isInAirAttack = false;
            currentGravity = characterGravity;
            verticalSpeed = -30f;

        }
        BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = -30f;
        BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = characterGravity;
    }
    public void SetJumpVelocityForDownBOSSS()
    {
        BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed -= 30f;
        BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = characterGravity;
    }
    public void SetJumpFallVelocity()
    {
        isInAirAttack = false;
        currentGravity = characterGravity;
        BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = characterGravity;
    }
    /// <summary>
    /// 角色重力
    /// </summary>
    void CaculateGravity()
    {
        //Debug.Log(isInAirAttack);
        if (CharacterInputSystem.Instance.playerJump && _Controller.isGrounded && _PlayableDirector.playableAsset.name.Contains("_Jump"))
        {
            ////isInAirAttack = false;
            //if (!_PlayableDirector.playableAsset.name.Contains("_Jump"))
            //{
            //    isReadyJump = true;
            //}
            //else
            {
                verticalSpeed = 8f; // 给角色一个初始向上的速度
            }
        }
        //if(isReadyJump && _PlayableDirector.playableAsset.name.Contains("_Jump"))
        //{
        //    verticalSpeed = 8f;
        //    isReadyJump = false;
        //}
        //角色是否站在地面（或某个碰撞体）上
        else if (_Controller.isGrounded)
        {
            
            isInAirAttack = false; // 一旦着地，退出空中攻击状态
            //将垂直速度归零
            //verticalSpeed = 0;
            //在地面时阻止速度无限下降
            if (verticalSpeed < 0.0f)
            {
                verticalSpeed = -5f;
            }
        }
        else
        {
            // 如果玩家按下攻击键且不在空中攻击状态，则进入空中攻击状态
            if (CharacterInputSystem.Instance.playerLAtk && !isInAirAttack)
            {
                isInAirAttack = true;
                verticalSpeed = 0f; // 在空中攻击时保持垂直速度为零
                //BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = 0f;
                //BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = 0f;
                inertia = Vector3.zero;
            }
            if (isInAirAttack)
            {
                // 如果处于空中攻击状态，不受重力影响
                verticalSpeed = 0f;
                //BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = 0f;
                //BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = 0f;

                inertia = Vector3.zero;
            }
            else
            {
                //否则V = gt  (Time.deltaTime表示“上一帧与当前帧间隔的秒数”既“一帧的时间”)
                if (verticalSpeed <= 0)
                {
                    //如果是下降阶段，此时的加速度是上升阶段的1.3倍
                    verticalSpeed += currentGravity * 5f * Time.deltaTime;
                    BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed += currentGravity * 5f * Time.deltaTime;


                }
                if (verticalSpeed < -30)
                {
                    verticalSpeed = -30;
                    BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = -30;

                }
                else
                {
                    verticalSpeed += currentGravity * Time.deltaTime;
                    BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed += currentGravity * Time.deltaTime;
                }

            }
        }


        //Debug.Log(verticalSpeed);
    }

    public virtual void AirNotAttack()
    {
        isInAirAttack = false;
    }
    public virtual void FreeVelocity()
    {
        float tempGravity = 0f;
        currentGravity = tempGravity;
        verticalSpeed = 0f; // 在空中攻击时保持垂直速度为零
        BOSS.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = 0f;
        BOSS.GetComponent<CharacterMoveMentControllerBase>().currentGravity = tempGravity;
    }

    /// <summary>
    /// 移动接口
    /// </summary>
    /// <param name="moveDirection">移动方向</param>
    /// <param name="moveSpeed">移动速度</param>
    /// /// <param name="useGravity">是否使用重力</param>
    public virtual void CharacterMoveInterface(Vector3 moveDirection, float moveSpeed, bool useGravity)
    {
        //如果移动方向的前方没有障碍物
        if (!CanAnimationMotion())
        {
            //移动方向标准化
            movementDirection = moveDirection.normalized;

            //对当前移动方向进行坡度检测
            //movementDirection = ResetMoveDirectionOnSlop(movementDirection);

            //如果使用重力
            if (useGravity)
            {
                //给垂直向量Y轴赋值
                //verticalDirection.Set(0.0f, verticalSpeed, 0.0f);
            }
            else
            {
                //归零
                verticalDirection = Vector3.zero;
            }

            //移动
            _Controller.Move((moveSpeed * Time.deltaTime) * movementDirection + Time.deltaTime * verticalDirection);
        }
    }





}
