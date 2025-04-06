using UnityEngine;

public class PlayerLockOn : MonoBehaviour
{
    public Transform boss;
    public float rotationSpeed = 5f;
    private bool isLockedOn = true;

    void Update()
    {
        if (isLockedOn && boss != null)
        {
            Vector3 direction = boss.position - transform.position;
            direction.y = 0; // 让角色只在水平方向旋转
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
