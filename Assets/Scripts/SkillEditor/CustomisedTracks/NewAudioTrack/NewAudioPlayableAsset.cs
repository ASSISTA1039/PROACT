using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Assista.SkillEditor
{
    [System.Serializable]
    public class NewAudioPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        public AudioClip[] audioClips;
        [Range(0, 1)] public float volume = 1.0f; // Ìí¼ÓÒôÁ¿¿ØÖÆ

        public ClipCaps clipCaps => ClipCaps.Looping | ClipCaps.Extrapolation;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<NewAudioPlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();

            if (audioClips.Length > 0)
            {
                behaviour.audioClip = audioClips[Random.Range(0, audioClips.Length)];
            }

            behaviour.volume = volume; 

            return playable;
        }
    }
}
