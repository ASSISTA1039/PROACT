using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace Assista.SkillEditor
{
    [Serializable]
    public class VFXPlayableBehavior : PrefabControlPlayable
    {
        public Transform transform;
        public GameObject prefabGameObject;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            prefabGameObject.transform.position = transform.position;
            prefabGameObject.transform.rotation = transform.rotation;
            //Debug.Log("transform:" + transform.position);
            //Debug.Log("prefabGameObject.transform.position:" + prefabGameObject.transform.position);

            base.OnBehaviourPlay(playable, info);
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
        }




    }
}