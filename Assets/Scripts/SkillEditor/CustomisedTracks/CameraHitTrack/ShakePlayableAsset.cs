using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class ShakePlayableAsset : PlayableAsset
{
    public float shakeForce;

    // 绑定数据到 PlayableBehaviour
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ShakePlayableBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.shakeForce = shakeForce;
        return playable;
    }
}
