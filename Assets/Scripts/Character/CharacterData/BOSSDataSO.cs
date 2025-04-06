using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSSDataSO : CharacterDataBaseSO
{
    //失衡值
    public float MaxUnbalance;
    //当前韧性值
    public float Unbalance;

    void Start()
    {
        //初始化血量
        //maxHealth = 50000f;
        health = maxHealth;

        Unbalance = MaxUnbalance;


    }

    void Update()
    {

    }
}
