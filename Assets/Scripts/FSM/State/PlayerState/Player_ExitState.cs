using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "ExitState", menuName = "StateMachine/State/Player/New ExitState")]
    public class Player_ExitState : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Exit;
        [SerializeField] protected Player_JumpState _player_JumpState;

        //[SerializeField, Header("敌人检测")] private float detectionRang;
        //[SerializeField] protected LayerMask enemyLayer;

        //缓存检测到的敌人目标
        //private Collider[] detectionedTarget = new Collider[1];

        public override void OnEnter()
        {
            _PlayableDirector.Play(Exit);
            _PlayableDirector.extrapolationMode = isLoop;
            _player_JumpState.isFalling = false;
            _player_JumpState.isLanding = false;
        }

        public override void OnExit()
        {
            _StateMachineSystem.currentTarget = null;

        }

        public override void OnUpdate()
        {
            


        }



    }
}

