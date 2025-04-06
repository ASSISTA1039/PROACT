using System.Collections;
using System.Collections.Generic;
using Assista.FSM;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace Assista.SkillEditor
{
    public class AttackDetectionPlayableAsset_Enemy : PlayableAsset, ITimelineClipAsset
    {
        AttackDetectionPlayableBehaviour_Enemy attackDetection_enemy = new AttackDetectionPlayableBehaviour_Enemy();

        //¹¥»÷¼ì²â
        [SerializeField, Header("¹¥»÷¼ì²â")] private ExposedReference<Transform>  attackDetectionCenter;
        [SerializeField] private Vector3 Position;

        [SerializeField] public DetectionType DetectionType;
        [SerializeField] public DetectionShape DetectionShape;

        [SerializeField] private Vector3 CubeSize;
        [SerializeField] private float attackDetectionRang;
        [SerializeField] private LayerMask enemyLayer;

        [SerializeField] private string HitAnimationName;
        [SerializeField] private AudioClip[] HitAudios;

        [SerializeField] private GameObject HitVFX;
        //[SerializeField] public Transform Player;

        [SerializeField] private bool AttackAdsorption;

        public float RotationTime;

        private GameObject GameObject;


        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }
        


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var scriptPlayable = ScriptPlayable<AttackDetectionPlayableBehaviour_Enemy>.Create(graph, attackDetection_enemy);
            AttackDetectionPlayableBehaviour_Enemy playable = scriptPlayable.GetBehaviour();

            //playable.InputSystem = owner.GetComponentInParent<CharacterInputSystem>();
            playable.stateMachine = owner.GetComponent<EnemyStateMachine>();
            playable.MoveController = owner.GetComponent<CharacterMoveMentControllerBase>();
            playable._Player = owner.transform;
            playable.attackDetectionCenter = attackDetectionCenter.Resolve(graph.GetResolver());
            playable.attackDetectionRang = attackDetectionRang;
            playable.enemyLayer = enemyLayer;
            playable.AttackAdsorption = AttackAdsorption;
            playable.RotationTime = RotationTime;
            //playable.gizmos = owner.GetComponent<DrawGizmos>();

            switch (DetectionType)
            {
                case DetectionType.AttackDetection:
                    playable.gizmos = attackDetectionCenter.Resolve(graph.GetResolver()).GetComponent<DrawGizmos>();

                    break;
                case DetectionType.SearchEnemies:

                    break;
            }

                    
            playable.Player = owner;
            playable.CubeSize = CubeSize;
            playable.Position = Position;
            playable.DetectionType = DetectionType;
            playable.DetectionShape = DetectionShape;
            playable.HitAudios = HitAudios;
            playable.HitAnimationName = HitAnimationName;
            playable.HitVFX = HitVFX;

            //playable.gameObject = GameObject;

            return scriptPlayable;
        }
    }

}
