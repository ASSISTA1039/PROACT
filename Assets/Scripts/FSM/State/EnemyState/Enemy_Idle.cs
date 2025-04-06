using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEditor.Timeline;


namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "Enemy_Idle", menuName = "StateMachine/State/Enemy/Enemy_Idle")]
    public class Enemy_Idle : StateBaseSO
    {
        [SerializeField] protected PlayableAsset Idle;


        public override void OnEnter()
        {
            _PlayableDirector.Play(Idle);
            _PlayableDirector.extrapolationMode = isLoop;

        }


        public override void OnUpdate()
        {
            Debug.Log("没发现敌人，待机");


        }
    }
}

