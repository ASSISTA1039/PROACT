using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
public class LerpTransformPlayableBehaviour : PlayableBehaviour
{
    public Transform target;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float duration;

    private float elapsed;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (target == null) return;

        elapsed += info.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);
        target.position = Vector3.Lerp(startPosition, endPosition, t);
    }
}
