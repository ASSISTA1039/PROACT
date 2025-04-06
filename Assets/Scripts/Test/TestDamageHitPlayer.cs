using Assista.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamageHitPlayer : MonoBehaviour
{
    public Transform pivot;
    // 旋转速度
    public float rotationSpeed = 50f;

    // 是否正在旋转
    private bool isRotating = false;

    public Collider player;
    void Update()
    {
        RotateDoor();
        
    }

    private void RotateDoor()
    {
        // 绕自身Y轴旋转
        transform.RotateAround(pivot.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 检测与玩家的碰撞
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("玩家与柱体发生了碰撞！");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // 玩家离开碰撞
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("玩家离开了柱体的碰撞范围！");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检测与玩家的碰撞
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家进入了柱体区域！");

            other.GetComponent<PlayerStateMachine>().isEnemyAttacked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 玩家离开柱体
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家离开了柱体区域！");
            other.GetComponent<PlayerStateMachine>().isEnemyAttacked = false;
        }
    }
}
