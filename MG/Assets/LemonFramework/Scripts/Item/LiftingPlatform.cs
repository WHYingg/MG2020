using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftingPlatform : MonoBehaviour
{
    public GameObject liftingPlatform;
    public Transform upPos;
    public Transform downPos;
    public float speed = 3f;
    private bool platformSwitch = false;

    void Start()
    {
    }


    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        platformSwitch = !platformSwitch;

        if (platformSwitch)
        {
            liftingPlatform.transform.DOMove(upPos.transform.position, speed);
        }
        else
        {
            liftingPlatform.transform.DOMove(downPos.transform.position, speed);
        }
    }
}
