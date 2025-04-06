using Assista;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.VFX;
using Assista.FSM;

public class CameraHitFeel : MonoBehaviour
{
    [SerializeField] public CinemachineImpulseSource cinemachineImpulseSource;
    [SerializeField] public PlayerStateMachine _playerStateMachine;
     //[SerializeField] private Camera_ZoomController zoomController;



    public void StartSlowTime(float timeScale)
    {

        Time.timeScale = timeScale;
    }
    public void EndSlowTime()
    {
        Time.timeScale = 1;
    }



    #region ΥπΖΑ
    public void CameraShake(float shakeForce)
    {
        if (shakeForce == 0) { return; }
        Debug.Log(shakeForce);
        if(gameObject.GetComponent<PlayerStateMachine>().AnyEnemyInRange())
        {
            cinemachineImpulseSource.GenerateImpulseWithForce(shakeForce);
        }
    }


    #endregion

    #region ZoomIn
    //float currentZoom;
    //public void ZoomIn(float distance)
    //{
    //    currentZoom = zoomController.currentDistance;
    //    zoomController.SetZoom(distance, 100);

    //}
    //public void ResetZoom()
    //{
    //    zoomController.SetZoom(currentZoom, 50);
    //}

    #endregion
}
