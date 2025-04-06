using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

namespace Assista.FSM
{
    public abstract class StateMachineBase : MonoBehaviour
    {
        //玩家控制器
        //[SerializeField, Header("玩家控制器")] public CharacterMoveMentControllerBase _Controller;
        public float boxCombatF_DistanceUpDown;
        public bool iswudi;
        //状态机当前状态
        [SerializeField, Header("当前状态")] public StateBaseSO CurrentState;

        [SerializeField, Header("所有状态")] private List<CharacterStates> States = new List<CharacterStates>();

        //用于存储所有状态的字典
        public Dictionary<string, StateBaseSO> StatesDictionary = new Dictionary<string, StateBaseSO>();
        //用于存储所有音效播放组件
        public Dictionary<string, AudioSource> AudioSourcesDictionary = new Dictionary<string, AudioSource>();
        public Transform Camera { get; set; }
        //敌人
        [SerializeField] public Transform currentTarget { get; set; }

        //索敌
        [SerializeField, Header("敌人检测")] public float detectionRang;
        [SerializeField] protected LayerMask enemyLayer;
        //缓存检测到的敌人目标
        [SerializeField] protected Collider[] detectionedTarget = new Collider[5];

        //固定对象BOSS
        [SerializeField] public Collider BOSSTarget;

        //相机公转
        [SerializeField] public PlayerCameraUtility playerCameraUtility;
        [field: SerializeField] public List<PlayerCameraRecenteringData> SidewaysCameraRecenteringData { get; private set; }

        [field: SerializeField] public List<PlayerCameraRecenteringData> BackWardsCameraRecenteringData { get; private set; }


        //进入Q技能大招，需要让BOSS保持静止
        public bool isBOSSStaticStop = false;



        //角色数值系统，暂时写在这
        //血量上限
        public float maxHealth;
        //当前血量
        public float health;

        public float MaxEnergy = 100f;
        public float energy = 0;
        public Image hpSlider; // 玩家 血条
        public Image enegySlider; // 玩家 时空能量条

        public float attack = 500f;

        protected void Awake()
        {
            InitCharacterStates();
            playerCameraUtility.Init();
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

        void Start()
        {
            MaxEnergy = 100f;
            energy = 0f;
        }

        void Update()
        {

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
                    StatesDictionary.Add(States[i].StateName, States[i].State);

                    States[i].State.InitState(this);

                }
            }
        }

        /// <summary>
        /// 状态转换
        /// </summary>
        /// <param name="toState"></param>
        public void BackLastState(String toState)
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



        /// <summary>
        /// 索定距离最近的一名敌人
        /// </summary>
        public void SeekTheEnemy()
        {
            //检测球体范围内的目标
            int targetCount = Physics.OverlapSphereNonAlloc(gameObject.transform.position, detectionRang, detectionedTarget, enemyLayer);
            //Debug.Log("检测球体范围内的目标:" + targetCount);


            if (targetCount > 0)
            {
                currentTarget = searchNearestTarget(targetCount).transform;
                //Debug.Log(detectionedTarget[0].transform.position);
            }
            else
            {
                currentTarget = null;
            }
        }


        /// <summary>
        /// 判断是否正在攻击敌人
        /// </summary>
        public bool AnyEnemyInRange()
        {
            //检测球体范围内的目标
            int targetCount = Physics.OverlapSphereNonAlloc(gameObject.transform.position, detectionRang, detectionedTarget, enemyLayer);
            if (targetCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 搜索检测到的敌人中距离最近的一名敌人
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




        [Serializable]
        private class CharacterStates
        {
            public string StateName;
            public StateBaseSO State;
        }


    }




}

