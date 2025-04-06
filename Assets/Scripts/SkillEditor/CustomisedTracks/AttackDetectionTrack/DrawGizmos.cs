using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    [HideInInspector] public float detectionRang;
    [HideInInspector] public Vector3 CubeSize;
    [HideInInspector] public DetectionShape DetectionShape;

    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (this.enabled)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;

            switch (DetectionShape)
            {
                case DetectionShape.spherical:
                    Gizmos.DrawWireSphere(Vector3.zero, detectionRang);

                    break;
                case DetectionShape.rectangle:
                    Gizmos.DrawWireCube(Vector3.zero, CubeSize);

                    break;
            }



        }
    }
}



