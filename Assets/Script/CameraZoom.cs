using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    public Rigidbody2D playerRb;
    public float lerpSpeed;

    private void LateUpdate()
    {
        cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, 12 + playerRb.velocity.magnitude, Time.deltaTime * lerpSpeed) ;
    }
}
