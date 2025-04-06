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
            direction.y = 0; // �ý�ɫֻ��ˮƽ������ת
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
