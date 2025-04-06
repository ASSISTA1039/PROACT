using Cinemachine;
using System.Collections;
using UnityEngine;

public class DollyController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera; // 绑定你的虚拟相机
    //[SerializeField] private float moveDuration = 2f; // 总路径移动时间
    [SerializeField] private Vector3 fixedRotation; // 固定初始旋转的角度

    private CinemachineTrackedDolly trackedDolly; // 跟踪的 Dolly
    private float elapsedTime;

    private void Start()
    {
        // 获取 Tracked Dolly 组件
        trackedDolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    public void StartPathMovement(float _moveduration)
    {
        elapsedTime = 0f;
        // 固定初始旋转
        if (virtualCamera != null)
        {
            //virtualCamera.transform.rotation = Quaternion.Euler(fixedRotation);
        }

        StartCoroutine(MoveAlongPath(_moveduration));
    }

    private IEnumerator MoveAlongPath(float moveDuration)
    {
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            // 动态更新 Path Position
            if (trackedDolly != null)
            {
                trackedDolly.m_PathPosition = Mathf.Lerp(0f, 1f, elapsedTime / moveDuration);
            }

            yield return null;
        }
    }
}
