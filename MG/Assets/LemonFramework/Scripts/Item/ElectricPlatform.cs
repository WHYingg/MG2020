using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPlatform : MonoBehaviour
{

    private bool isCanPass = false;
    public GameObject gear;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        isCanPass = true;
        gear.transform.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    private void OnTriggerExit(Collider other)
    {
        isCanPass = false;
    }
}
