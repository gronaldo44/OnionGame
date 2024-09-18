using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    public void UpdateCameraFollow(Transform playerTransform)
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = playerTransform;
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera is not assigned in CameraManager.");
        }
    }
}
