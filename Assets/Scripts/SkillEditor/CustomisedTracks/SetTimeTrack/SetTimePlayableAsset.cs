using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Animancer;
using Assista.FSM;

namespace Assista.SkillEditor
{
    public enum inputSystemEnum
    {
        Latk = 0,
        Move = 1,
        Evade = 2,
        E = 3,
        Q = 4,
        None = 5,
        NoDelay_E = 6,
        SetAtk = 7,
        LatkLong = 8,
        E_Long = 9,
        Jump = 10,
        F = 11,
        F_Long = 12,
        Latk_Air = 13,
        SetAtk_Air = 14,


    }
    public enum SkipType
    {
        SkipTime,
        SkipState,
        PlayerRotation,
        //BOSSSkipTime
    }
    public enum RotationType
    {
        MoveRotation,
        AinmationRotation

    }


    public class SetTimePlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        SetTimePlayableBehaviour SetTimePlayable = new SetTimePlayableBehaviour();
    
        //private CharacterInputSystem InputSystem;
        public inputSystemEnum inputEnum;
        public SkipType SkipType;
        public RotationType RotationType;

        //public StateBaseSO Idle;
        //public StateBaseSO Move;
        //public StateBaseSO Evade;

        public StateBaseSO Combat;
        //public StateMachineSystem StateMachine;
        public float SetTime;
        public float SetAtkTime;
        public float RotationTime;
        public bool isEditor;

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var scriptPlayable = ScriptPlayable<SetTimePlayableBehaviour>.Create(graph, SetTimePlayable);
            SetTimePlayableBehaviour playable = scriptPlayable.GetBehaviour();

            playable.Director = owner.GetComponent<PlayableDirector>();
            //playable.InputSystem = owner.GetComponentInParent<CharacterInputSystem>();
            playable.StateMachine = owner.GetComponent<StateMachineBase>();

            playable._Player = owner.transform.transform;
            playable.Animancer = owner.GetComponent<AnimancerComponent>();
            playable.RotationTime = RotationTime;
            playable.SetAtkTime = SetAtkTime;
        playable.SetTime = SetTime;
            playable.isEditor = isEditor;
            playable.RotationType = RotationType;
            playable.inputEnum = inputEnum;
            playable.SkipType = SkipType;
            //playable.Combat = Combat;
            //playable.Idle = Idle;
            //playable.Move = Move;
            //playable.Evade = Evade;

            return scriptPlayable;
        }
    }

}
