using System;
using UnityEditor;
using UnityEngine.Timeline;

namespace Assista.SkillEditor
{
    [TrackColor(127f/255, 214f / 255, 252f / 255)]
    [TrackClipType(typeof(VFXPlayableAsset))]
    public class VFXTrack : ControlTrack
    {
        
    }
}