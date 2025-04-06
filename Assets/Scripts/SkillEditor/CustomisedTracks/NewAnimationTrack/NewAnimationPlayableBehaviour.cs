using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using UnityEngine.Playables;

public class NewAnimationPlayableBehaviour : PlayableBehaviour
{
    public AnimancerComponent _Animancer;
    public ClipTransition _AnimationClip;
    private float _time;

    public override void OnGraphStart(Playable playable)
    {
        //Debug.Log("当所属图形开始播放时调用");
        if (_Animancer.States.Current != null)
        {
            //_Animancer.States.Current.Time = _time;
        }
    }

    public override void OnGraphStop(Playable playable)
    {
        //Debug.Log("暂停");

        //_Animancer = null;

        if (_Animancer && _Animancer.States.Current != null)
        {
            //_Animancer.States.Current.Time = _time;
        }

    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //Debug.Log("时间轴进入Clip片段");
        //当前播放状态为空或者当前播放的动画与_AnimationClip.Clip不同时，播放_AnimationClip
        if (_Animancer.States.Current == null || _Animancer.States.Current.Clip != _AnimationClip.Clip)
        {
            _Animancer.Playable.Speed = 2f;
            _AnimationClip.Speed = 2f;
            
            _Animancer.Play(_AnimationClip);
        }
        if (_Animancer.States.Current != null)
        {
            _Animancer.States.Current.Time = _time;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        //Debug.Log("时间轴离开Clip片段");
        
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        if (Application.isEditor)
        {
            base.PrepareFrame(playable, info);
            _Animancer.States.Current.Speed = 0f;
            _time = (float)playable.GetTime();
            _Animancer.States.Current.MoveTime(_time, false);
            //编辑动画时预览动画位移
            //OnValidate();

        }
    }

    public override void OnPlayableCreate(Playable playable)
    {
        //Debug.Log("动画片段创建时调用" + playable.GetDuration());
        
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        //Debug.Log("动画片段删除时调用" + playable.GetDuration());
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

    }

    public override void PrepareData(Playable playable, FrameData info)
    {
        base.PrepareData(playable, info);
    }

    private void OnValidate()
    {
        //AnimancerUtilities.EditModeSampleAnimation(_AnimationClip.Clip, _Animancer, _time * _AnimationClip.Clip.length);
        //_AnimationClip.Clip.EditModeSampleAnimation(_Animancer, _time * _AnimationClip.Clip.length);
        _AnimationClip.Clip.EditModeSampleAnimation(_Animancer, _time);
    }


}