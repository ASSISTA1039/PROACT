using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.Timeline;
using Assista.FSM;

namespace Assista.SkillEditor
{
    [System.Serializable]
    public class SpeedControlPlayable : PlayableAsset
    {
        public float playbackSpeed = 1f; // 自定义播放速度

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            // 创建 ScriptPlayable
            var playable = ScriptPlayable<SpeedControlPlayableBehaviour>.Create(graph);

            // 设置行为参数
            var behaviour = playable.GetBehaviour();
            behaviour.speed = playbackSpeed;
            behaviour.director = owner.GetComponent<PlayableDirector>();
            //Debug.Log(behaviour.director);
            return playable;
        }
    }
}