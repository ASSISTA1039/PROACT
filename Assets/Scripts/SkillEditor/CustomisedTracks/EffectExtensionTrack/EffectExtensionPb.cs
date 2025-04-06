using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

using System.Collections.Generic;


namespace Assista.SkillEditor
{
    [Serializable]
    public class EffectExtensionPb : PlayableBehaviour
    {
        public Transform EffectTransform;

        public EffectExtensionPa EffectExtensionPa;
        public GameObject prefabGameObject;
        public GameObject sourceObject;
        public PlayableGraph graph;
        public List<Playable> playables;

        //public GameObject go;
        //public PlayableAsset m_ControlDirectorAsset;

        public bool updateDirector;
        public bool updateParticle;
        public bool searchHierarchy;
        //public double m_Duration;
        public bool m_SupportLoop;
        public bool controllingParticles;
        public uint particleRandomSeed;


        private IList<ParticleSystem> particleSystems;

        public HashSet<ParticleSystem> s_SubEmitterCollector;
        const int k_MaxRandInt = 10000;
        public List<PlayableDirector> k_EmptyDirectorsList;
        public List<ParticleSystem> k_EmptyParticlesList;
        public HashSet<GameObject> s_CreatedPrefabs;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            //EffectExtensionPa.Transform = EffectTransform;

            if (prefabGameObject != null)
            {
                Transform parenTransform = sourceObject != null ? sourceObject.transform : null;

                if(EffectTransform != null)
                {
                    prefabGameObject.transform.position = EffectTransform.position;
                    prefabGameObject.transform.rotation = EffectTransform.rotation;
                }
                

                var controlPlayable = PrefabControlPlayable.Create(graph, prefabGameObject, parenTransform);

                sourceObject = controlPlayable.GetBehaviour().prefabInstance;
                sourceObject.SetActive(true);
                playables.Add(controlPlayable);
            }

            if (sourceObject != null)
            {
                var directors = updateDirector ? GetComponent<PlayableDirector>(sourceObject) : k_EmptyDirectorsList;
                particleSystems = updateParticle ? GetControllableParticleSystems(sourceObject) : k_EmptyParticlesList;

                /*UpdateDurationAndLoopFlag(directors, particleSystems);

                var director = go.GetComponent<PlayableDirector>();
                if (director != null)
                    m_ControlDirectorAsset = director.playableAsset;
*/



                if (updateParticle)
                    SearchHierarchyAndConnectParticleSystem(particleSystems, graph, playables);
                //playable = ConnectPlayablesToMixer(graph, playables);

            }
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (particleSystems == null) return;
            foreach (var particleSystem in particleSystems)
            {
                if (particleSystem != null)
                {
                    particleSystem.Simulate((float)playable.GetTime(), true, true, false);
                }
            }
        }
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            prefabGameObject.transform.position = Vector3.zero;
            prefabGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            if (particleSystems == null) return;
            foreach (var particleSystem in particleSystems)
            {
                if (particleSystem != null)
                {
                    particleSystem.Simulate(0, true, true, false);
                }
            }
        }



        /*internal void UpdateDurationAndLoopFlag(IList<PlayableDirector> directors, IList<ParticleSystem> particleSystems)
        {
            if (directors.Count == 0 && particleSystems.Count == 0)
                return;

            const double invalidDuration = double.NegativeInfinity;

            var maxDuration = invalidDuration;
            var supportsLoop = false;

            foreach (var director in directors)
            {
                if (director.playableAsset != null)
                {
                    var assetDuration = director.playableAsset.duration;

                    //if (director.playableAsset is TimelineAsset && assetDuration > 0.0)
                    // Timeline assets report being one tick shorter than they actually are, unless they are empty
                    //assetDuration = (double)((DiscreteTime)assetDuration).OneTickAfter();

                    maxDuration = Math.Max(maxDuration, assetDuration);
                    supportsLoop = supportsLoop || director.extrapolationMode == DirectorWrapMode.Loop;
                }
            }

            foreach (var particleSystem in particleSystems)
            {
                maxDuration = Math.Max(maxDuration, particleSystem.main.duration);
                supportsLoop = supportsLoop || particleSystem.main.loop;
            }

            m_Duration = double.IsNegativeInfinity(maxDuration) ? PlayableBinding.DefaultDuration : maxDuration;
            m_SupportLoop = supportsLoop;
        }*/

        static Playable ConnectPlayablesToMixer(PlayableGraph graph, List<Playable> playables)
        {
            var mixer = Playable.Create(graph, playables.Count);

            for (int i = 0; i != playables.Count; ++i)
            {
                ConnectMixerAndPlayable(graph, mixer, playables[i], i);
            }

            mixer.SetPropagateSetTime(true);

            return mixer;
        }

        static void ConnectMixerAndPlayable(PlayableGraph graph, Playable mixer, Playable playable,
            int portIndex)
        {
            graph.Connect(playable, 0, mixer, portIndex);
            mixer.SetInputWeight(playable, 1.0f);
        }

        void SearchHierarchyAndConnectParticleSystem(IEnumerable<ParticleSystem> particleSystems, PlayableGraph graph,
            List<Playable> outplayables)
        {
            foreach (var particleSystem in particleSystems)
            {
                if (particleSystem != null)
                {
                    //controllingParticles = true;
                    
                    outplayables.Add(ParticleControlPlayable.Create(graph, particleSystem, particleRandomSeed));
                    particleSystem.Play();
                }
            }
        }

        internal IList<T> GetComponent<T>(GameObject gameObject)
        {
            var components = new List<T>();
            if (gameObject != null)
            {
                if (searchHierarchy)
                {
                    gameObject.GetComponentsInChildren<T>(true, components);
                }
                else
                {
                    gameObject.GetComponents<T>(components);
                }
            }
            return components;
        }


        IList<ParticleSystem> GetControllableParticleSystems(GameObject go)
        {
            var roots = new List<ParticleSystem>();

            // searchHierarchy will look for particle systems on child objects.
            // once a particle system is found, all child particle systems are controlled with playables
            // unless they are subemitters

            if (searchHierarchy || go.GetComponent<ParticleSystem>() != null)
            {
                GetControllableParticleSystems(go.transform, roots, s_SubEmitterCollector);
                s_SubEmitterCollector.Clear();
            }

            return roots;
        }

        static void GetControllableParticleSystems(Transform t, ICollection<ParticleSystem> roots, HashSet<ParticleSystem> subEmitters)
        {
            var ps = t.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                if (!subEmitters.Contains(ps))
                {
                    roots.Add(ps);
                    CacheSubEmitters(ps, subEmitters);
                }
            }

            for (int i = 0; i < t.childCount; ++i)
            {
                GetControllableParticleSystems(t.GetChild(i), roots, subEmitters);
            }
        }

        static void CacheSubEmitters(ParticleSystem ps, HashSet<ParticleSystem> subEmitters)
        {
            if (ps == null)
                return;

            for (int i = 0; i < ps.subEmitters.subEmittersCount; i++)
            {
                subEmitters.Add(ps.subEmitters.GetSubEmitterSystem(i));
                // don't call this recursively. subEmitters are only simulated one level deep.
            }
        }




    }
}