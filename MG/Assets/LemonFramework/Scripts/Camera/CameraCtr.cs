using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraCtr : MonoBehaviour
{
    public enum CameraType
    {
        Perspective,
        Orthographic
    }
    public CameraType cameraType;
    public Transform player1;
    public Transform player2;
    [Header("大小开始跟随变化距离")]
    public float targetDistance;
    [Header("摄像机大小跟随系数")]
    public float sizeMultiplier = 0.4f;
    [Header("摄像机大小最大跟随距离")]
    public float maxDistance = 100f;
    private Vector3 offset;
    private Camera camera;
    private float size;
    void Start()
    {
        offset = transform.position - (player1.position + player2.position) / 2;
        camera = GetComponent<Camera>();
        if (cameraType == CameraType.Perspective)
            size = camera.fieldOfView;
        else
            size = camera.orthographicSize;

    }

    private void LateUpdate()
    {
        transform.position = new Vector3((player1.position.x + player2.position.x) / 2 + offset.x, transform.position.y, transform.position.z);
        float distance = Mathf.Abs(player1.position.x - player2.position.x);

        if (cameraType == CameraType.Perspective)
        {
            if (distance >= targetDistance && distance <= maxDistance)
                camera.DOFieldOfView(distance * sizeMultiplier, 1).SetUpdate(UpdateType.Normal, true);
            else
                camera.DOFieldOfView(size, 1).SetUpdate(UpdateType.Normal, true);
        }
        else
        {
            if (distance >= targetDistance && distance <= maxDistance)
                camera.DOOrthoSize(distance * sizeMultiplier, 1).SetUpdate(UpdateType.Normal, true);
            else
                camera.DOOrthoSize(size, 1).SetUpdate(UpdateType.Normal, true);
        }

    }
}
