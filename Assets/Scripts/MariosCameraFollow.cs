using Cinemachine;
using DilmerGames.Core.Singletons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MariosCameraFollow : Singleton<MariosCameraFollow>
{
    [SerializeField]
    private float AmplitudeGain = 0.5f;
    [SerializeField]
    private float FrequencyGain = 0.5f;
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    public void FollowPlayer(Transform transform)
    {
        // not all scenes have a cinemachine virtual camera so return in that's the case
        //test
        if (cinemachineVirtualCamera == null)
            return;
        cinemachineVirtualCamera.Follow = transform;
        var perlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = AmplitudeGain;
        perlin.m_FrequencyGain = FrequencyGain;
    }
}
