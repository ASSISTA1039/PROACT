using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using Animancer;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.InputSystem.XR;
using Cinemachine.Utility;

namespace Assista.FSM
{
    public class EnemyStateMachine : MonoBehaviour, IDamagar
    {
        //状态机当前状态
        [SerializeField, Header("当前状态")] public EnemyStateBase CurrentStatePrefab;
        private EnemyStateBase CurrentState;

        [SerializeField, Header("所有状态")] private List<CharacterStates> States = new List<CharacterStates>();

        private EnemyIdleState idleState;
        private EnemyDieState DieState;
        private EnemyHitState HitState;
        private EnemyWalkState WalkState;
        private EnemyCombatState CombatState;
        private EnemyRangeCombatState RangeCombatState;
        private EnemyDisapperChargeState DisapperChargeState;
        private EnemyKnockDownState KnockDownState;
        private EnemyAirHitState AirHitState;


        //BOSS二阶段开始时进入烟雾
        public GameObject fog;


        //用于存储所有状态的字典
        public Dictionary<string, EnemyStateBase> StatesDictionary = new Dictionary<string, EnemyStateBase>();
        //用于存储所有音效播放组件
        public Dictionary<string, AudioSource> AudioSourcesDictionary = new Dictionary<string, AudioSource>();
        public Transform Camera { get; set; }


        //索敌
        [SerializeField] private Transform detectionCenter;
        [SerializeField, Header("敌人检测")] private float detectionRang;
        //玩家
        [SerializeField] protected LayerMask whatisPlayer;
        [SerializeField] protected LayerMask whatisGround; //障碍物
        //缓存检测到的敌人目标
        [SerializeField] private Collider[] detectionedTarget = new Collider[1];
        [SerializeField, Header("目标")] public Transform currentTarget;
        public BOSSDataSO dataSO;

        //角色数值系统，暂时写在这
        //血量上限
        public float maxHealth;
        //当前血量
        public float health;

        public Image hpSlider; // Boss 血条
        public Image staggerSlider; // Boss 韧度条

        private CharacterController Controller;
        private Camera mainCamera;
        [SerializeField] private Transform damagarNumericalValueTransform;
        public Transform Hit;
        //public float BossToughen = 100f;
        public bool RefreshToughen = false;
        private Coroutine toughenRecoveryCoroutine;

        //BOSS被击中的次数
        public int hitCount = 0;
        public bool 已经倒地 = false;

        public bool BOSS被砸到地面 = false;
        //闪现到玩家背后，暂时隐藏BOSS        //BOSS的材质mesh
        public GameObject body;
        public Material targetMaterial;
        public Material originMaterial;
        public bool isDisapperCharge;
        public GameObject player;


        //玩家Q技能控制BOSS的开关门
        public bool isopen;



        protected void Awake()
        {
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            dataSO = GetComponent<BOSSDataSO>();
            dataSO.Unbalance = dataSO.MaxUnbalance;
            RefreshToughen = false;
            已经倒地 = false;
            isDisapperCharge = false;

            idleState = new EnemyIdleState();
            HitState = new EnemyHitState();
            DieState = new EnemyDieState();
            WalkState = new EnemyWalkState();
            CombatState = new EnemyCombatState();
            RangeCombatState = new EnemyRangeCombatState();
            DisapperChargeState = new EnemyDisapperChargeState();
            KnockDownState = new EnemyKnockDownState();
            AirHitState = new EnemyAirHitState();
            AIView();
            InitCharacterStates();

            //InitCharacteraudioSource();

            /*//初始化血量
            maxHealth = 50000f;
            health = maxHealth;*/
            
            CurrentState = CurrentStatePrefab.GetComponent<EnemyStateBase>();

            idleState.Paste(CurrentState.Copy());
            idleState?.OnEnter();

            //hpSlider.fillAmount = dataSO.maxHealth;
            //staggerSlider.fillAmount = dataSO.MaxUnbalance;

            isopen = false;
        }

        void Start()
        {
            Controller = transform.GetComponentInParent<CharacterController>();
            BackLastState("Idle");
        }

        void Update()
        {
            if(dataSO.health <= 0)
            {
                BackLastState("idle");
            }
            AIView();
            CurrentState?.OnUpdate();

            hpSlider.fillAmount = dataSO.health/dataSO.maxHealth;
            staggerSlider.fillAmount = dataSO.Unbalance/dataSO.MaxUnbalance;

            // 如果需要恢复韧性，并且协程尚未运行，则启动恢复协程
            if (RefreshToughen && toughenRecoveryCoroutine == null)
            {
                toughenRecoveryCoroutine = StartCoroutine(RecoverToughen());
            }
        }

        // 韧性恢复的协程
        private IEnumerator RecoverToughen()
        {
            float recoverSpeed = 20f; // 每秒恢复多少韧性
            float maxToughen = dataSO.MaxUnbalance;  // 设定最大韧性值

            while (dataSO.Unbalance < maxToughen && RefreshToughen)
            {
                dataSO.Unbalance += recoverSpeed * Time.deltaTime;
                dataSO.Unbalance = Mathf.Min(dataSO.Unbalance, maxToughen); // 限制最大值
                yield return null; // 等待下一帧
            }

            // 恢复完成后，清空协程变量
            toughenRecoveryCoroutine = null;
            RefreshToughen = false;
            已经倒地 = false;
        }

        //初始化状态机
        private void InitCharacterStates()
        {
            if (States.Count == 0) return;
            //首先遍历外面配置的资源
            for (int i = 0; i < States.Count; i++)
            {
                //检查列表第一个元素的内容是否已经在池子里面了，没有的话就创建一个
                if (!StatesDictionary.ContainsKey(States[i].StateName))
                {
                    //EnemyStateBase enemyState = gameObject.GetComponent<EnemyStateBase>();
                    //Debug.Log("States[i].State:" + States[i].State);

                    //States[i].State.Copy()[];

                    //Type type = Type.GetType(States[i].State.name);
                    //Type IdleType = idleState.GetType();
                    //Debug.Log("type:" + type);
                    //Debug.Log("IdleType:" + IdleType);


                    Debug.Log(Type.GetType(States[i].State.name));
                    Debug.Log(idleState.GetType());
                    if (Type.GetType(States[i].State.name) == idleState.GetType())
                    {
                        idleState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, idleState);
                        idleState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == DieState.GetType())
                    {
                        DieState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, DieState);
                        DieState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == HitState.GetType())
                    {
                        HitState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, HitState);
                        HitState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == WalkState.GetType())
                    {
                        WalkState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, WalkState);
                        WalkState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == RangeCombatState.GetType())
                    {
                        RangeCombatState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, RangeCombatState);
                        RangeCombatState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == CombatState.GetType())
                    {
                        CombatState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, CombatState);
                        CombatState.InitState(this);
                    }
                    if (Type.GetType(States[i].State.name) == DisapperChargeState.GetType())
                    {
                        DisapperChargeState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, DisapperChargeState);
                        DisapperChargeState.InitState(this);
                    }                    
                    if (Type.GetType(States[i].State.name) == KnockDownState.GetType())
                    {
                        KnockDownState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, KnockDownState);
                        KnockDownState.InitState(this);
                    }                    
                    if (Type.GetType(States[i].State.name) == AirHitState.GetType())
                    {
                        AirHitState.Paste(States[i].State.Copy());
                        StatesDictionary.Add(States[i].StateName, AirHitState);
                        AirHitState.InitState(this);
                    }
                    //StatesDictionary.Add(States[i].StateName, States[i].State);

                    //States[i].State.InitState(this);

                }
            }
        }

        private void InitCharacteraudioSource()
        {
            Transform parent = transform.parent.GetComponent<Transform>();
            //获取到所有的音效播放组件存储在字典中
            if (parent != null)
            {
                //遍历父物体的所有子物体
                foreach (Transform child in parent)
                {
                    //跳过当前GameObject自身
                    if (child != transform)
                    {
                        //遍历兄弟物体的所有子物体
                        foreach (Transform subChild in child)
                        {
                            AudioSource audioSource = subChild.GetComponent<AudioSource>();
                            if (audioSource != null)
                            {
                                //检查列表第一个元素的内容是否已经在池子里面了，没有的话就创建一个
                                if (!AudioSourcesDictionary.ContainsKey(audioSource.name))
                                {
                                    AudioSourcesDictionary.Add(audioSource.name, audioSource);
                                }
                                //Debug.Log("name:" + audioSource.name);
                            }
                        }
                    }
                }
            }
        }



        /// <summary>
        /// 状态转换
        /// </summary>
        /// <param name="toState"></param>
        public void BackLastState(string toState)
        {
            if (!StatesDictionary.ContainsKey(toState))
            {
                Debug.Log("没有找到“" + toState + "”状态！");
                return;
            }

            //执行旧状态的结束事件
            CurrentState?.OnExit();
            //替换新状态
            CurrentState = StatesDictionary[toState];
            //执行新状态的开始事件
            CurrentState?.OnEnter();
        }


        //
        /// <summary>
        /// 索定玩家方向
        /// </summary>


        /// <summary>
        /// 搜索检测到的敌人中距离最近的一名玩家
        /// </summary>
        /// <returns></returns>
        private Collider searchNearestTarget(int targetCount)
        {
            //初始化一个“最近距离”为无穷大,和一个用于储存最近目标的变量
            float closestDistance = Mathf.Infinity;
            Collider closestCollider = null;
            //遍历数组，尝试更换距离更近的目标
            for (int i = 0; i < targetCount; i++)
            {
                //Debug.Log("碰撞体中心点: " + detectionedTarget[0].bounds.center);

                float distance = Vector3.Distance(detectionedTarget[i].transform.position, transform.position);
                //如果这个距离小于之前找到的最近距离
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = detectionedTarget[i];
                    //Debug.Log(closestCollider.transform.position);
                }
            }
            return closestCollider;
        }

        /// <summary>
        /// 索定距离最近的一名玩家
        /// </summary>
        public void SeekThePlayer()
        {
            //检测球体范围内的目标
            int targetCount = Physics.OverlapSphereNonAlloc(gameObject.transform.position, detectionRang, detectionedTarget, whatisPlayer);
            //Debug.Log("检测球体范围内的目标:" + targetCount);


            if (targetCount > 0)
            {
                currentTarget = searchNearestTarget(targetCount).transform;
                //Debug.Log(detectionedTarget[0].transform.position);
            }
            else
            {
                //currentTarget = null;
            }
        }

        /// <summary>
        /// 搜索检测玩家是否进入视野范围
        /// </summary>
        /// <returns></returns>
        public void AIView()
        {
            // 获取检测到的目标数量
            int targetCount = Physics.OverlapSphereNonAlloc(detectionCenter.position, detectionRang, detectionedTarget, whatisPlayer);

            if (targetCount > 0)
            {
                Transform player = detectionedTarget[0].transform;
                Vector3 enemyPos = transform.parent.position;
                Vector3 toPlayer = (player.position - enemyPos).normalized;

                // 计算玩家与敌人的距离
                float distanceToPlayer = Vector3.Distance(enemyPos, player.position);

                // 如果玩家与敌人的距离小于 2f，使用 CheckSphere 来检测
                if (distanceToPlayer < 2f)
                {
                    // 画出敌人检测的圆形范围（绿色）
                    Debug.DrawRay(enemyPos, toPlayer * 2f, Color.green);

                    // 使用 CheckSphere 进行圆形范围检测
                    Collider[] detected = Physics.OverlapSphere(enemyPos, 2f, whatisPlayer);
                    if (detected.Length > 0)
                    {
                        currentTarget = player;
                        Debug.Log("玩家在圆形范围内！");
                    }
                }
                else
                {
                    // 画出从敌人到玩家的射线（红色）
                    Debug.DrawRay(enemyPos + transform.parent.up * 0.5f, toPlayer * detectionRang, Color.red);

                    // 检测障碍物
                    if (!Physics.Raycast(enemyPos + transform.parent.up * 0.5f, toPlayer, out var hit, detectionRang, whatisGround))
                    {
                        float dot = Vector3.Dot(toPlayer, transform.parent.forward);
                        //Debug.Log($"玩家与敌人方向点积: {dot}");

                        // 画出敌人的前方向（蓝色）
                        Debug.DrawRay(enemyPos, transform.parent.forward * 2f, Color.blue);

                        if (dot > 0.25f)
                        {
                            currentTarget = player;
                            Debug.Log("玩家进入视野范围！");
                        }
                    }
                }
            }
        }

        public void DisapperCharge_Combat_Ground()
        {
            body.SetActive(false);
            isDisapperCharge = true;
            //StartCoroutine(LongPressWarp());
            float forwardOffset = -4f; // 前方的偏移量
            Vector3 newPosition = player.transform.position - player.transform.right * forwardOffset;
            newPosition.y = transform.parent.position.y; // 调整Y轴高度
            Debug.Log(newPosition);
            transform.parent.position = newPosition;
            // 使用 DOMove 方法移动角色，持续 0.1 秒
            //yield return transform.parent.DOMove(newPosition, 0.1f).OnComplete(() => {
            Vector3 targetDirection = (player.transform.position).normalized;
            targetDirection.y = 0f;
            transform.parent.rotation = Quaternion.LookRotation(targetDirection);
            body.SetActive(true);
        }

        public void DisapperCharge_Combat_Ground_2stage()
        {
            //StartCoroutine(LongPressWarp());
            Vector3 newPosition = player.transform.position - player.transform.forward * 5f;
            newPosition.y = transform.parent.position.y; // 调整Y轴高度
            transform.parent.position = newPosition;
            // 使用 DOMove 方法移动角色，持续 0.1 秒
            //yield return transform.parent.DOMove(newPosition, 0.1f).OnComplete(() => {
            Vector3 targetDirection = (player.transform.position).normalized;
            targetDirection.y = 0f;
            transform.parent.rotation = Quaternion.LookRotation(targetDirection);
        }

        public void DisapperCharge_Combat_Ground_2stage_End()
        {
            //StartCoroutine(LongPressWarp());
            Vector3 newPosition = player.transform.position - player.transform.forward * 20f;
            newPosition.y = transform.parent.position.y; // 调整Y轴高度
            transform.parent.position = newPosition;
            // 使用 DOMove 方法移动角色，持续 0.1 秒
            //yield return transform.parent.DOMove(newPosition, 0.1f).OnComplete(() => {
            Vector3 targetDirection = (player.transform.position).normalized;
            targetDirection.y = 0f;
            transform.parent.rotation = Quaternion.LookRotation(targetDirection);
        }

        //private IEnumerator LongPressWarp()
        //{
        //    if (currentTarget)
        //    {
        //        // 获取BOSS的朝向和位置
        //        Transform playerTransform = currentTarget.transform;
        //        Vector3 playerPosition = playerTransform.position;
        //        Vector3 playerForward = playerTransform.forward; // player的朝向
        //        Vector3 playerRight = playerTransform.right; // player的右侧方向
        //        Debug.Log(playerTransform);

        //        // 根据朝向计算传送点
        //        float forwardOffset = 30f; // 前方的偏移量
        //        float verticalOffset = playerPosition.y; // 垂直方向偏移量
        //        //float lateralOffset = -100f; // 横向偏移量，可自行调整

        //        //Vector3 newPosition = playerPosition + playerForward * forwardOffset + playerRight * lateralOffset;
        //        //newPosition.y = 0f; // 调整Y轴高度

        //        Vector3 directionToPlayer = (currentTarget.transform.position - transform.parent.position).normalized;

        //        Vector3 newPosition = player.transform.position - player.transform.forward * forwardOffset;
        //        newPosition.y = transform.parent.position.y; // 调整Y轴高度
        //        Debug.Log(newPosition);
        //        transform.parent.position = newPosition;
        //        // 使用 DOMove 方法移动角色，持续 0.1 秒
        //        //yield return transform.parent.DOMove(newPosition, 0.1f).OnComplete(() => {
        //        Vector3 targetDirection = (currentTarget.transform.position).normalized;
        //        targetDirection.y = 0f;
        //        transform.parent.rotation = Quaternion.LookRotation(targetDirection);
        //            //transform.parent.rotation = transform.parent.LockOnTarget(currentTarget, transform.parent, 200f);

        //            //body.SetActive(true);
        //        //}); // 等待完成

        //    }
        //}

        public void SetBoolDisapperCharge()
        {
            isDisapperCharge = false;
        }

        public void TakeDamager(float damagar, string hitAnimationName, Transform attacker, AudioClip audioClip, GameObject gameObject)
        {
            StatesDictionary["Hit"].String = hitAnimationName;
            StatesDictionary["Hit"].Clip = audioClip;
            BackLastState("Hit");
            //GameObjectPoolSystem.Instance.GameObjectPoolAdd(gameObject, Hit);
            Debug.Log(damagar);
            //受击时面向攻击者
            transform.parent.rotation = transform.parent.LockOnTarget(attacker, transform.parent, 100f);
            //扣血
            dataSO.health -= damagar * UnityEngine.Random.Range(1, 2);

            //GameObjectPoolSystem.Instance.TakeGameObject("damagarNumericalValue", damagarNumericalValueTransform).GetComponent<damagarNumericalValue>().Create(damagar * UnityEngine.Random.Range(1, 2), mainCamera);
            if (dataSO.health <= 0)
            {
                BackLastState("Die");

            }
        }

        public void TakeDamager_NoSound(float damager, string hitAnimation, Transform attacker)
        {
            hitCount++;

            //GameObjectPoolSystem.Instance.GameObjectPoolAdd(gameObject, Hit);
            if (isDisapperCharge) { return; }
            Debug.Log(damager);
            //受击时面向攻击者
            if (RefreshToughen && 已经倒地 && gameObject.GetComponent<CharacterMoveMentControllerBase>()._Controller.isGrounded)
            { }
            else
            {
                transform.parent.rotation = transform.parent.LockOnTarget(attacker, transform.parent, 200f);
            }

                        //扣血
                        if (!RefreshToughen)
            {
                if (attacker.GetComponent<PlayerStateMachine>().isRecordForPODUN)
                {
                    dataSO.Unbalance -= 30f;
                    attacker.GetComponent<PlayerStateMachine>().energy += 30f;
                }
                else
                {
                    dataSO.Unbalance -= 5f;
                    attacker.GetComponent<PlayerStateMachine>().energy += 1f;
                }
                attacker.GetComponent<PlayerStateMachine>().isRecordForPODUN = false;
                dataSO.health -= 4 * damager * UnityEngine.Random.Range(1, 2);
            }
            //身体材质换红表示扣血
            body.GetComponent<SkinnedMeshRenderer>().materials[0].color = Color.red;
            StartCoroutine(FlashRed());

            if (dataSO.Unbalance <= 0f && !RefreshToughen)
            {
                attacker.GetComponent<PlayerStateMachine>().energy += 20f;
            }

            if (dataSO.Unbalance <= 0.1f && !RefreshToughen)
            {
                dataSO.health -= 60f * damager * UnityEngine.Random.Range(1, 2);
                StatesDictionary["Hit"].String = hitAnimation;
                //让BOSS材质变白
                //body.GetComponent<SkinnedMeshRenderer>().material = targetMaterial;
                BackLastState("Hit");
                //body.GetComponent<SkinnedMeshRenderer>().material = originMaterial;
                //BOSS韧度开始恢复
                RefreshToughen = true;
            }
            if (RefreshToughen && 已经倒地)
            {
                if(!gameObject.GetComponent<CharacterMoveMentControllerBase>()._Controller.isGrounded)
                {
                    dataSO.health -= 35f * damager * UnityEngine.Random.Range(1, 2);
                    if(BOSS被砸到地面)
                    {
                        BOSS被砸到地面 = false;
                        Debug.Log("BOSS被砸到地面");
                        gameObject.GetComponent<CharacterMoveMentControllerBase>().verticalSpeed = -30f;
                        gameObject.GetComponent<CharacterMoveMentControllerBase>().currentGravity = -10f;
                    }
                    StatesDictionary["AirHit"].String = "AirHit";
                    BackLastState("AirHit");
                }
                else
                {
                    dataSO.health -= 20f * damager * UnityEngine.Random.Range(1, 2);
                    StatesDictionary["KnockDown"].String = hitAnimation;
                    BackLastState("KnockDown");
                }
            }

            
            //BOSS血量为0 死亡
            //if (dataSO.health <= 0)
            //{
            //    BackLastState("Die");
            //}
        }

        public void BOSSAirHitDown()
        {
            dataSO.health -= 40f * 20f * UnityEngine.Random.Range(1, 2);

            StatesDictionary["AirHit"].String = "AirHitDown";
            BackLastState("AirHit");
        }

        IEnumerator FlashRed()
        {
            // 1. 变红

            yield return new WaitForSeconds(0.05f);
            body.GetComponent<SkinnedMeshRenderer>().materials[0].color = Color.white;

        }

        public Transform GetCurrentTarget()
        {
            if(currentTarget == null)
            {
                currentTarget = GameObject.FindWithTag("Player").transform;
            }
            return currentTarget;
        }

        public float GetCurrentTargetDistance() => Vector3.Distance(currentTarget.position, transform.parent.position);

        [System.Serializable]
        private class CharacterStates
        {
            public string StateName;
            public EnemyStateBase State;
        }
    }
}

