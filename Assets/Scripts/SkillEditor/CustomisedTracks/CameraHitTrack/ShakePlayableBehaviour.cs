using UnityEngine;
using UnityEngine.Playables;

public class ShakePlayableBehaviour : PlayableBehaviour
{
    public float shakeForce;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // 获取绑定的 CameraHitFeel 脚本实例
        var cameraHitFeel = playerData as CameraHitFeel;
        if (cameraHitFeel != null)
        {
            Debug.Log("111");
            cameraHitFeel.CameraShake(shakeForce);
        }
    }
}
