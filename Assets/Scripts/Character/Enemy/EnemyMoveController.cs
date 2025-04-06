using Assista.FSM;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EnemyMoveController : CharacterMoveMentControllerBase
{
    public Transform Enemy;

    protected override void Start()
    {
        //_Animancer = GetComponent<AnimancerComponent>();
        _Controller = GetComponentInParent<CharacterController>();

    }

    protected override void Update()
    {
        base.Update();
        if(gameObject.GetComponent<EnemyStateMachine>().BOSS被砸到地面)
        {
            currentGravity = characterGravity*100f;
            verticalSpeed = -900f;
            gameObject.GetComponent<EnemyStateMachine>().BOSS被砸到地面 = false;
        }
    }

    private void OnAnimatorMove()
    {
        _Controller.Move(_Animancer.Animator.deltaPosition + Time.deltaTime * new Vector3(0.0f, verticalSpeed, 0.0f));

        Enemy.transform.rotation *= _Animancer.Animator.deltaRotation;
    }


}
