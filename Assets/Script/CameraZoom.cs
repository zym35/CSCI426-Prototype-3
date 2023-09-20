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
    public float shakeDuration;

    private void LateUpdate()
    {
        cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, 12 + playerRb.velocity.magnitude, Time.deltaTime * lerpSpeed) ;
    }

    public IEnumerator Shake(float power)
    {
        cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = power;
        yield return new WaitForSeconds(shakeDuration);
        cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }
}
