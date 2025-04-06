using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitExpandingFunction
{
    /// <summary>
    /// 锁定目标方向
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="target"></param>
    /// <param name="self"></param>
    /// <param name="lerpTime"></param>
    /// <returns></returns>
    public static Quaternion LockOnTarget(this Transform transform, Transform target, Transform self, float lerpTime)
    {
        if (target == null) return self.rotation;

        Vector3 targetDirection = (target.position - self.position).normalized;
        targetDirection.y = 0f;

        Quaternion newRotation = Quaternion.LookRotation(targetDirection);

        return Quaternion.Lerp(self.rotation, newRotation, lerpTime * Time.deltaTime);
        //return Quaternion.Lerp(self.rotation, newRotation, lerpTime);
    }


}
