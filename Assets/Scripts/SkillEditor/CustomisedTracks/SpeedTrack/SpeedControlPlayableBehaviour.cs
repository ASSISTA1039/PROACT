using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Animancer;
using Assista.FSM;

namespace Assista.SkillEditor
{
    public class SpeedControlPlayableBehaviour : PlayableBehaviour
    {
        public PlayableDirector director; // 持有 PlayableDirector 的引用
        public float speed = 1f;          // 目标速度

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            // 每帧检查并设置速度
            UpdateSpeed();
        }

        /// <summary>
        /// 更新 PlayableGraph 的速度
        /// </summary>
        private void UpdateSpeed()
        {
            if (director != null)
            {
                PlayableGraph graph = director.playableGraph;
                // 确保 PlayableGraph 存在
                if (graph.IsValid() && graph.GetRootPlayable(0).IsValid())
                {
                    //Debug.Log(graph.GetRootPlayable(0));
                    graph.GetRootPlayable(0).SetSpeed(speed);
                }
            }
        }
    }
}