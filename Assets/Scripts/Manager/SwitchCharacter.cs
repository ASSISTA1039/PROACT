using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assista.FSM;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.TextCore.Text;
using Cinemachine;

public class SwitchCharacter : MonoBehaviour
{
    [System.Serializable]
    public class SwicthCharacterInfo
    {
        public string characterName; // 角色的名称
        public Vector3 newTransfrom; // 新角色位置
        public Transform lookAtPos; // 视角位置
        public Transform followAtPos; // 视角位置
        public GameObject character; // 角色对象

        [HideInInspector] public Animator animator; // 参与初始化，方便播放每个各个角色的动画
        //[HideInInspector] public CharacterInputSystem characterInputSystem;
        [HideInInspector] public CharacterController Controller;
        [HideInInspector] public PlayerStateMachine StateMachine;
    }

    [SerializeField, Header("所有角色")] private List<SwicthCharacterInfo> swicthCharacterInfos = new List<SwicthCharacterInfo>(); // 所有角色的信息列表

    private Queue<SwicthCharacterInfo> Characters = new Queue<SwicthCharacterInfo>();

    private bool canSwichInput; // 是否可以切换角色
    //[SerializeField, Header("切换角色的缓冲时间")] private float applyNextSwitchTime;
    [SerializeField, Header("退场角色的存留时间")] private float switchOutCharacterTime;

    private SwicthCharacterInfo currentCharacter;//当前角色
    private SwicthCharacterInfo oldCharacter;// 旧角色

    [SerializeField, Header("相机")] private CinemachineVirtualCamera Camera; // 虚拟相机，更新相机目标点

    [SerializeField] public float groundCheckDistance = 0.1f;
    [SerializeField] public LayerMask groundLayer;


    //角色1开大招时切换角色2，触发角色2大招
    public bool isCombat_Q = false;

    // 在Awake方法中初始化虚拟相机和角色信息
    private void Awake()
    {
        InitCharacter();
    }

    // 在InitCharacter方法中获取所有角色的动画控制器、输入系统和受击脚本
    private void InitCharacter()
    {
        for (int i = 0; i < swicthCharacterInfos.Count; i++)
        {
            swicthCharacterInfos[i].animator = swicthCharacterInfos[i].character.transform.GetComponentInChildren<Animator>();

            //swicthCharacterInfos[i].characterInputSystem = swicthCharacterInfos[i].character.transform.GetComponent<CharacterInputSystem>();

            swicthCharacterInfos[i].Controller = swicthCharacterInfos[i].character.transform.GetComponent<CharacterController>();

            swicthCharacterInfos[i].StateMachine = swicthCharacterInfos[i].character.transform.GetComponentInChildren<PlayerStateMachine>();
            //swicthCharacterInfos[i].StateMachine.Awake();
            //swicthCharacterInfos[i].StateMachine.Start();

            Characters.Enqueue(swicthCharacterInfos[i]);

            swicthCharacterInfos[i].character.SetActive(false);
        }

    }

    // 在Start方法中设置初始的角色
    private void Start()
    {
        canSwichInput = true;
        //将1号位角色出队并激活
        currentCharacter = Characters.Dequeue();
        currentCharacter.character.SetActive(true);
        //摄像机跟随
        Camera.LookAt = currentCharacter.lookAtPos;
        Camera.Follow = currentCharacter.followAtPos;

    }

    // 在Update方法中检查是否需要切换角色
    private void Update()
    {
         SwtichInput();
    }

    private void SwtichInput()
    {
        // 如果不能切换角色，直接返回
        if (!canSwichInput) return;
        // 切换角色的输入
        switch (CharacterInputSystem.Instance.playerSwitchType)
        {
            case 1:
                // 禁止角色切换
                canSwichInput = false;

                oldCharacter = currentCharacter;
                //oldCharacter.characterInputSystem.enabled = false;

                oldCharacter.StateMachine.BackLastState("Exit");


                CharacterSwtich(oldCharacter);
                Characters.Enqueue(oldCharacter);
                oldCharacter.character.SetActive(false);
                // 开始协程
                StartCoroutine(MyCoroutine(oldCharacter));
                break;
            case 2:
                // 禁止角色切换
                canSwichInput = false;

                oldCharacter = currentCharacter;
                //oldCharacter.characterInputSystem.enabled = false;
                oldCharacter.StateMachine.BackLastState("Exit");

                CharacterSwtich(oldCharacter);

                Characters.Enqueue(oldCharacter);
                oldCharacter.character.SetActive(false);
                // 开始协程
                StartCoroutine(MyCoroutine(oldCharacter));
                break;
            default:
                //Debug.Log("未知类型");
                break;

        }
    }

    //协程，一定时间后将旧角色入队并隐藏
    IEnumerator MyCoroutine(SwicthCharacterInfo Character)
    {
        yield return new WaitForSeconds(switchOutCharacterTime);
        canSwichInput = true;


    }

    private void CharacterSwtich(SwicthCharacterInfo Character)
    {
        currentCharacter = Characters.Dequeue();

        currentCharacter.character.SetActive(true);
        //输入控制
        //currentCharacter.characterInputSystem.enabled = true;
        //摄像机跟随
        Camera.LookAt = currentCharacter.lookAtPos;
        Camera.Follow = currentCharacter.followAtPos;
        currentCharacter.character.GetComponent<CharacterMoveMentControllerBase>().isInAirAttack = false;
        //当旧角色刚释放完大招，并且新角色的大招能量也满了的时候
        if (isCombat_Q)
        {
            currentCharacter.StateMachine.BackLastState("Combat_Q");
            isCombat_Q = false;
        }
        else
        {
            //否则，正常情况下
            if (CheckGrounded(Character))
            {
                currentCharacter.StateMachine.BackLastState("Admission");
            }
            else
            {
                currentCharacter.StateMachine.BackLastState("Jump");
            }
        }



        // 新角色位置
        Vector3 oldPosition = Character.character.transform.position;
        Vector3 offset = Character.character.transform.forward * currentCharacter.newTransfrom.z +
                         Character.character.transform.right * currentCharacter.newTransfrom.x;

        // 如果旧角色在空中，将新角色的位置设置为同一高度
        Vector3 newPosition = oldPosition + offset;
        if (!Character.Controller.isGrounded)
        {
            newPosition.y = oldPosition.y; // 保持新角色的高度与旧角色一致
        }
        currentCharacter.Controller.Move(newPosition - currentCharacter.character.transform.position);
        //新角色旋转
        currentCharacter.character.transform.rotation = Character.character.transform.rotation;

        //// 将旧角色的垂直速度传递给新角色
        //var oldVerticalSpeed = Character.Controller.velocity.y;
        //var newRigidbody = currentCharacter.character.GetComponent<Rigidbody>();
        //if (newRigidbody != null)
        //{
        //    newRigidbody.velocity = new Vector3(newRigidbody.velocity.x, oldVerticalSpeed, newRigidbody.velocity.z);
        //}
    }



    private bool CheckGrounded(SwicthCharacterInfo Character)
    {
        return Physics.Raycast(Character.character.transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

}
