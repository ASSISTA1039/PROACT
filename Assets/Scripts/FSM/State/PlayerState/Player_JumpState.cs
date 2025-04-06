using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.ProBuilder.Shapes;

namespace Assista.FSM
{
    [CreateAssetMenu(fileName = "JumpState", menuName = "StateMachine/State/Player/New JumpState")]
    public class Player_JumpState : StateBaseSO
    {
        [SerializeField] private PlayableAsset Jump;
        [SerializeField] private PlayableAsset Falling;
        [SerializeField] private PlayableAsset Land;

        public bool isFalling = false;
        public bool isLanding = false;

        private float jumpForce = 5f;
        private float gravity = -9.8f; 
        private float verticalVelocity;

        private bool isGrounded;
        private float playerTransformY;
        private float deltaY; 
        [SerializeField] public float groundCheckDistance = 0.1f;
        [SerializeField] public LayerMask groundLayer;
        [SerializeField] public Player_AirCombatState player_AirCombatState;
        [SerializeField] private float playbackSpeed = 5f;
        public override void OnEnter()
        {
            //Debug.Log(isGrounded);
            groundCheckDistance = 0.1f;
            playerTransformY = _Player.transform.position.y;
            PerformJump();
            if (_PlayableDirector != null)
            {
                _PlayableDirector.stopped += OnTimelineFinished;
            }
        }

        public override void OnExit()
        {
            if (_PlayableDirector != null)
            {
                _PlayableDirector.stopped -= OnTimelineFinished;
            }

            ResetState();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            ApplyGravity();
            deltaY = _Player.transform.position.y - playerTransformY;
            //Debug.Log(deltaY);
            //if(deltaY > 2f)
            //{
            //    groundCheckDistance = 0.5f;
            //}
            if (!isFalling && !CheckGrounded() && playerTransformY > 1f)
            {
                //Debug.Log(isFalling);
                HandleFalling();
            }
            else if (isLanding && CheckGrounded())
            {
                //Debug.Log(isLanding);
                LandAction();
            }
            if (!CheckGrounded() && CharacterInputSystem.Instance.playerLAtk)
            {
                _StateMachineSystem.BackLastState("AirCombat");
            }
            if (CharacterInputSystem.Instance.Combat_F)
            {
                _StateMachineSystem.BackLastState("Combat_F");
            }
            if (CharacterInputSystem.Instance.playerSlide)
            {
                _StateMachineSystem.BackLastState("Evade");
            }
            playerTransformY = _Player.transform.position.y;
        }

        private void PerformJump()
        {
            
            verticalVelocity = jumpForce;
            if(player_AirCombatState.isAirCombat || !CheckGrounded())
            {
                //isFalling = true;
                HandleFalling();
                player_AirCombatState.isAirCombat = false;
            }
            else if(CheckGrounded() && !player_AirCombatState.isAirCombat)
            {
                _PlayableDirector.Play(Jump);
            }
            _PlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0.8f * playbackSpeed);
            if (_PlayableDirector != null)
            {
                
                _PlayableDirector.extrapolationMode = DirectorWrapMode.None;
            }
        }

        private void ApplyGravity()
        {
            if (isFalling && !CheckGrounded())
            {
                verticalVelocity += gravity * Time.deltaTime;
                Vector3 moveVector = new Vector3(0, verticalVelocity, 0);
                _Player.position += moveVector * Time.deltaTime;
            }
            else
            {
                if (!isLanding && isFalling)
                {
                    isLanding = true;
                }
            }
        }

        private void HandleFalling()
        {
            if (!isFalling)
            {
                isFalling = true;
                //Debug.Log(_PlayableDirector.playableAsset);
                if (_PlayableDirector != null && _PlayableDirector.playableAsset != Falling)
                {
                    _PlayableDirector.Play(Falling);
                    _PlayableDirector.extrapolationMode = DirectorWrapMode.Hold;
                }
            }
        }

        private void LandAction()
        {
            isFalling = false;
            isLanding = false;
            groundCheckDistance = 0.1f;
            if (_PlayableDirector != null && _PlayableDirector.playableAsset != Land)
            {
                _PlayableDirector.Play(Land);
                _PlayableDirector.extrapolationMode = DirectorWrapMode.None;
                playerTransformY = 0f;
                groundCheckDistance = 0.1f;
            }
        }

        private bool CheckGrounded()
        {
            if(isFalling && playerTransformY >= 2f)
            {
                groundCheckDistance = 1f;
            }
            return Physics.Raycast(_Player.transform.position, Vector3.down, groundCheckDistance, groundLayer);
        }

        private void ResetState()
        {
            //isFalling = false;
            //isLanding = false;
            //verticalVelocity = 0;
        }

        private void OnTimelineFinished(PlayableDirector director)
        {
            _PlayableDirector.Stop();
            //if (isLanding)
            //{
            //    _StateMachineSystem.BackLastState("Idle");
            //}
        }
    }
}
