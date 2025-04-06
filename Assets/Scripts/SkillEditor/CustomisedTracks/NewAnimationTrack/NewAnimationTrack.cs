using System.Collections;
using System.Collections.Generic;
using Animancer;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace Assista.SkillEditor
{
    [TrackColor(60 / 255f, 205 / 255f, 173 / 255f)]
    [TrackClipType(typeof(NewAnimationPlayableAsset))]
    //[TrackBindingType(typeof(AnimancerComponent))]

    public class NewAnimationTrack : TrackAsset//, AnimationTrack
    {
        /*NewAnimancerMixerBehaviour mixerBehaviour = new NewAnimancerMixerBehaviour();

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var scriptPlayable = ScriptPlayable<NewAnimancerMixerBehaviour>.Create(graph, mixerBehaviour);

            NewAnimancerMixerBehaviour playable = scriptPlayable.GetBehaviour();
            //获取到AnimancerComponent组件
            //playable._Animancer = go.GetComponent<AnimancerComponent>();
            //Debug.Log("go.GetComponent<AnimancerComponent>():" + go.GetComponent<AnimancerComponent>());

            return scriptPlayable;
        }*/


    }
}

