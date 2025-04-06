using UnityEngine;
using UnityEngine.Playables;

namespace Assista.SkillEditor
{
    public class NewAudioPlayableBehaviour : PlayableBehaviour
    {
        public AudioClip audioClip;
        public float volume = 1.0f; // ƒ¨»œ“Ù¡ø

        private AudioSource audioSource;

        public override void OnGraphStart(Playable playable)
        {
            if (audioSource == null)
            {
                GameObject audioObject = new GameObject("TimelineAudio");
                audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (audioSource != null && audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.volume = volume;
                audioSource.Play();
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (audioSource != null)
            {
                GameObject.Destroy(audioSource.gameObject);
            }
        }
    }
}
