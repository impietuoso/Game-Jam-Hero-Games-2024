using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour {
    public static CameraShake instance;
    private CinemachineVirtualCamera _cam;
    public float shakeIntensity = 1;
    public float shakeTime = 0.2f;

    float currentTime;
    CinemachineBasicMultiChannelPerlin _cbmcp;

    void Awake()     {
        instance = this;
        _cam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start() {
        StopShake();
    }

    public void Shake() {
        CinemachineBasicMultiChannelPerlin _cbmcp = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = shakeIntensity;

        currentTime = shakeTime;
    }

    public void StopShake() {
        CinemachineBasicMultiChannelPerlin _cbmcp = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0f;

        currentTime = 0;
    }


    private void Update() {
        if (currentTime > 0) {
            currentTime -= Time.deltaTime;

            if(currentTime <= 0) {
                StopShake();
            }
        }
    }
}
