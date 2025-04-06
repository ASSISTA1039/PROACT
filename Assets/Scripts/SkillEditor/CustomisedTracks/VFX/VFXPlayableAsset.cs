using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Assista.SkillEditor
{
    [Serializable]
    [NotKeyable]
    public class VFXPlayableAsset : ControlPlayableAsset
    {
        VFXPlayableBehavior vFX = new VFXPlayableBehavior();
        public ExposedReference<Transform> transform;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            //prefabGameObject.transform.position = transform.Resolve(graph.GetResolver()).position;
            //prefabGameObject.transform.rotation = transform.Resolve(graph.GetResolver()).rotation;
            //Debug.Log("go:" + go.transform.position);
            //var scriptPlayable = base.CreatePlayable(graph, go);
            //var playable = scriptPlayable.GetBehaviour();

            var scriptPlayable1 = ScriptPlayable<VFXPlayableBehavior>.Create(graph, vFX);
            VFXPlayableBehavior playable1 = scriptPlayable1.GetBehaviour();
            playable1.transform = transform.Resolve(graph.GetResolver());
            playable1.prefabGameObject = prefabGameObject;


            var controlPlayable = PrefabControlPlayable.Create(graph, prefabGameObject, sourceGameObject.Resolve(graph.GetResolver()).transform);
            var playable2 = controlPlayable.GetBehaviour();
            //playable2.


            //PrefabControlPlayable.
            //return scriptPlayable1;
            return controlPlayable;
            //return base.CreatePlayable(graph, go);
        }

        /*private void OnEnable()
        {
            base.OnEnable();
            Debug.Log("ppppppppppppppppppppp");
        }*/


    }
}
