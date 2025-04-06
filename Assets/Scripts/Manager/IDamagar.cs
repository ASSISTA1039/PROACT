using UnityEngine;

//接口，传递受击方的伤害（扣血量）、受击动画和受击位移
public interface IDamagar
{
    //伤害值、受伤动画、攻击者、受击音效、受击特效
    void TakeDamager(float damagar, string hitAnimationName, Transform attacker, AudioClip audioClip, GameObject gameObject);
    void TakeDamager_NoSound(float damagar, string hitAnimationName, Transform attacker);
}
