using Assista.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Playables;
using static SwitchCharacter;

public class PlayerMoveController : CharacterMoveMentControllerBase
{
    public Transform _Player;
    private float finalSpeed;
    [Header("Acceleration/Deceleration")]
    public float acceleration = 5f;
    public float deceleration = 7f;
    public float targetSpeed = 0f;
    private Vector3 moveDirection;
    //private Vector3 currentVelocity = Vector3.zero;

    [SerializeField] public float groundCheckDistance = 0.1f;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public SwicthCharacterInfo character;
    //���״̬��
    public PlayerStateMachine _StateMachine;


    //���ƽ�ɫ�˶�״̬�µĹ�ת
    public new Transform camera;
    

    private void Awake()
    {
    }
    protected override void Start()
    {
        base.Start();
        _StateMachine = GetComponent<PlayerStateMachine>();
    }

    protected override void Update()
    {
        base.Update();
        
        // ��ȡ���뷽��
        //Vector2 input = CharacterInputSystem.Instance.playerMovement;
        //Vector3 moveDirection = new Vector3(input.x, 0, input.y);

        //// ��������룬���ƶ���ɫ
        //if (moveDirection.sqrMagnitude > 0.01f)
        //{
        //    // ��ǰ�ٶ�ȡ�����ܲ�״̬
        //    float currentSpeed = CharacterInputSystem.Instance.playerRun ? runSpeed : walkSpeed;

        //    // �ƶ���ɫ
        //    CharacterMoveInterface(moveDirection, currentSpeed, true);
        //}

        //��RootMotion���ƽ�ɫ�ƶ�
        //HandleMovement();
    }



    //private void HandleMovement()
    //{
    //    if (_StateMachine.CurrentState == null) return;
    //    moveDirection.x = CharacterInputSystem.Instance.playerMovement.x;
    //    moveDirection.z = CharacterInputSystem.Instance.playerMovement.y;
    //    // ���ݵ�ǰ״̬��ȡĿ���ٶ�
    //    float targetSpeed = _StateMachine.CurrentState.GetTargetSpeed();
    //    // ��ֵ�����ٶ�
    //    float speedChange = (targetSpeed > currentVelocity.magnitude ? acceleration : deceleration) * Time.deltaTime;
    //    currentVelocity = Vector3.Lerp(currentVelocity, moveDirection * targetSpeed, speedChange);
    //    //Debug.Log(CharacterInputSystem.Instance.playerMovement);
    //    // Ӧ��λ��


    //    //_Controller.gameObject.transform.position += currentVelocity * Time.deltaTime;
    //}

    private void OnAnimatorMove()
    {
        if (_StateMachine.canAnimMotion)
        {
            if (!CanAnimationMotion())
            {
                //_Animancer.Animator.applyRootMotion = true;
                //float currentRootY = _Animancer.Animator.rootPosition.y;
                //Debug.Log(verticalSpeed);
                //Debug.Log(_PlayableDirector.playableAsset.name);
                if (!_PlayableDirector.playableAsset.name.Contains("Jump"))
                {
                    //Debug.Log(_PlayableDirector.playableAsset.name);
                    inertia = _Animancer.Animator.deltaPosition;
                    _Controller.Move(_Animancer.Animator.deltaPosition + Time.deltaTime * new Vector3(0.0f, verticalSpeed, 0.0f));


                }
                else
                {
                    moveDirection.x = CharacterInputSystem.Instance.playerMovement.x;
                    moveDirection.z = CharacterInputSystem.Instance.playerMovement.y;

                    _Controller.Move((0.5f * inertia) + Time.deltaTime * new Vector3(0f, verticalSpeed, 0f));
                }
                //transform.rotation = _Animancer.Animator.deltaRotation;
                //Debug.Log(CharacterInputSystem.Instance.playerJump);
                //_Controller.Move(currentVelocity * Time.deltaTime);
                //_Player.transform.LookAt(transform.position + GetRelativeDiretion(CharacterInputSystem.Instance.playerMovement));
            }
        }
        //_Player.transform.rotation *= _Animancer.Animator.deltaRotation;

    }

    public override void AirNotAttack()
    {
        base.AirNotAttack();
    }

    private bool CheckGrounded()
    {
        return Physics.Raycast(_Player.transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}
