using System.Collections;
using System.Collections.Generic;
using Assista.FSM;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.SkillEditor
{
    public class AttackDetectionPlayableBehaviour : PlayableBehaviour
    {
        public PlayerStateMachine stateMachine;
        //public CharacterInputSystem InputSystem;
        public CharacterMoveMentControllerBase MoveController;
        private Vector3 movementDirection;
        public Transform _Player;
        //敌人
        //public Transform currentTarget;

        public Transform attackDetectionCenter;
        //public GameObject gameObject;
        public float attackDetectionRang;
        public LayerMask enemyLayer;
        public GameObject Player;
        public DetectionType DetectionType;
        public DetectionShape DetectionShape;
        public Vector3 CubeSize;
        public Vector3 Position;
        public DrawGizmos gizmos;

        public string HitAnimationName;
        public GameObject HitVFX;
        public AudioClip[] HitAudios;

        public bool AttackAdsorption;
        private float t = 0.0f;
        //转身时间（用于调整转身速度）
        public float RotationTime;

        //用于与检测攻击检测重叠的碰撞体，最多检测4个
        Collider[] attackDetectionTargets = new Collider[4];

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            //Debug.Log("时间轴进入Clip片段");

            switch (DetectionType)
            {
                case DetectionType.AttackDetection:
                    //gizmos视图辅助线
                    gizmos.transform.localPosition = Position;
                    switch (DetectionShape)
                    {
                        case DetectionShape.spherical:
                            gizmos.DetectionShape = DetectionShape.spherical;
                            break;
                        case DetectionShape.rectangle:
                            gizmos.DetectionShape = DetectionShape.rectangle;
                            break;
                    }
                    //Debug.Log(AttackDetectionType());
                    //攻击检测
                    //Debug.Log("检测到的碰撞体的个数:" + AttackDetectionType());
                    if (AttackDetectionType() > 0)
                    {
                        for (int i = 0; i < AttackDetectionType(); i++)
                        {
                            //在碰撞体所在物体的子物体中尝试获取接口
                            if(enemyLayer.Equals("Player"))
                            {
                                if (attackDetectionTargets[i].GetComponent<PlayerStateMachine>().TryGetComponent(out IDamagar damagar))
                                {
                                    //Debug.Log("敌人:" + attackDetectionTargets[i].GetComponentInChildren<PlayerStateMachine>());
                                    //Debug.Log("敌人接口:" + damagar);

                                    //damagar.TakeDamager(500f, HitAnimationName, Player.transform, HitAudios[Random.Range(0, HitAudios.Length)], HitVFX);
                                    damagar.TakeDamager_NoSound(500f, HitAnimationName, Player.transform.parent);
                                }
                            }
                            else
                            {
                                if (attackDetectionTargets[i].GetComponentInChildren<EnemyStateMachine>().TryGetComponent(out IDamagar damagar))
                                {
                                    //Debug.Log("敌人:" + attackDetectionTargets[i].GetComponentInChildren<PlayerStateMachine>());
                                    //Debug.Log("敌人接口:" + damagar);

                                    //damagar.TakeDamager(500f, HitAnimationName, Player.transform, HitAudios[Random.Range(0, HitAudios.Length)], HitVFX);
                                    damagar.TakeDamager_NoSound(20f, HitAnimationName, Player.transform);
                                }
                            }

                        }
                    }

                    break;
            }

        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            switch (DetectionType)
            {
                case DetectionType.AttackDetection:
                    gizmos.enabled = true;
                    gizmos.detectionRang = attackDetectionRang;
                    gizmos.CubeSize = CubeSize;

                    break;
                case DetectionType.SearchEnemies:

                    float deltaTime = Time.deltaTime;
                    t += deltaTime / RotationTime;
                    //防止t超过1
                    t = Mathf.Clamp01(t);
                    if (stateMachine.currentTarget == null)
                    {
                        if (CharacterInputSystem.Instance.playerMovement.sqrMagnitude > Mathf.Epsilon)
                        {
                            //角色在地面，且输入不为零，且可以移动，则赋予移动速度
                            movementDirection.z = CharacterInputSystem.Instance.playerMovement.x;
                            movementDirection.x = CharacterInputSystem.Instance.playerMovement.y;
                            //记录摄像机的旋转角度
                            float angleOfRotaion = Camera.main.transform.rotation.eulerAngles.y;

                            Quaternion quaternion = Quaternion.Euler(0, angleOfRotaion, 0) * Quaternion.Euler(0, Vector3.SignedAngle(movementDirection, new Vector3(1, 0, 0), _Player.up), 0);

                            _Player.rotation = Quaternion.Slerp(_Player.rotation, quaternion, t);

                        }
                    }
                    else
                    {
                        EnemyStateMachine enemystate = Player.GetComponent<EnemyStateMachine>();
                        Debug.Log(enemystate);
                        Debug.Log(Player);

                        if (enemystate != null)
                        {
                            Debug.Log("here is player");
                            _Player.parent.rotation = _Player.parent.LockOnTarget(Player.GetComponent<EnemyStateMachine>().currentTarget, _Player.parent.transform, 50f);
                        }
                        else
                        {
                            _Player.rotation = _Player.LockOnTarget(stateMachine.currentTarget, _Player.transform, 50f);
                        }
                    }

                    if ((stateMachine.currentTarget != null) && AttackAdsorption)
                    {
                        //Vector3 targetDirection;
                        //确定方向
                        //targetDirection = currentTarget.position - transform.root.position;

                        {
                            Vector3 targetDirection = stateMachine.currentTarget.position - _Player.position;
                            MoveController.CharacterMoveInterface(new Vector3(stateMachine.currentTarget.position.x - _Player.position.x, 0, stateMachine.currentTarget.position.z - _Player.position.z), 30f, true);
                        }
                    }


                    
                    break;
            }   


        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            //Debug.Log("时间轴离开Clip片段");
            switch (DetectionType)
            {
                case DetectionType.AttackDetection:
                    gizmos.enabled = false;
                    attackDetectionCenter.localPosition = new Vector3(0, 0, 0);
                    gizmos.transform.position = attackDetectionCenter.position;

                    break;
            }

        }


        /// <summary>
        /// 返回检测到的碰撞体的个数
        /// </summary>
        /// <returns></returns>
        private int AttackDetectionType()
        {
            switch (DetectionShape)
            {
                case DetectionShape.spherical:
                    return Physics.OverlapSphereNonAlloc(gizmos.transform.position, attackDetectionRang, attackDetectionTargets, enemyLayer);

                case DetectionShape.rectangle:
                    return Physics.OverlapBoxNonAlloc(gizmos.transform.position, CubeSize, attackDetectionTargets, gizmos.transform.rotation, enemyLayer);

                default:
                    return 0;
            }
        }





    }
}

