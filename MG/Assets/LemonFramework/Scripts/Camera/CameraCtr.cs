using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraCtr : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    [Header("大小开始跟随变化距离")]
    public float targetDistance;
    [Header("摄像机大小跟随系数")]
    public float sizeMultiplier = 0.4f;
    private Vector3 offset;
    private Camera camera;
    private float size;
    void Start()
    {
        offset = transform.position - (player1.position + player2.position) / 2;
        camera = GetComponent<Camera>();
        size = camera.orthographicSize;
    }

    private void LateUpdate()
    {
        transform.position = (player1.position + player2.position) / 2 + offset;
        float distance = Vector3.Distance(player1.position, player2.position);
        if (distance >= targetDistance)
            camera.DOOrthoSize(distance * sizeMultiplier, 1).SetUpdate(UpdateType.Normal,true);
        else
            camera.DOOrthoSize(size, 1).SetUpdate(UpdateType.Normal, true);
    }
}
