using UnityEngine;
using UnityEngine.Playables;

namespace Assista.SkillEditor
{
    [System.Serializable]
    public class LerpTransformPlayableAsset : PlayableAsset
    {
        public ExposedReference<Transform> target;
        public float _duration;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<LerpTransformPlayableBehaviour>.Create(graph);

            var behaviour = playable.GetBehaviour();
            behaviour.target = target.Resolve(graph.GetResolver());
            if (behaviour.target != null)
            {
                behaviour.endPosition = behaviour.target.position;
                
            }
            behaviour.startPosition = owner.transform.position;
            behaviour.duration = _duration;

            return playable;
        }
    }
}
