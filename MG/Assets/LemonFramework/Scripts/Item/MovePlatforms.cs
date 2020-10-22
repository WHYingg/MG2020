using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovePlatforms : MonoBehaviour
{
    public GameObject gear;
    public List<GameObject> cubs;
    public List<Transform> cubsPos;
    private bool isUserd = false;

    
    void Start()
    {
        foreach (var item in cubs)
        {
            item.SetActive(false);
        }
    }

    
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isUserd)
        {
            gear.transform.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            for (int i = 0; i < cubs.Count; i++)
            {
                cubs[i].SetActive(true);
                cubs[i].transform.DOMove(cubsPos[i].position, 3f);
            }
        }

    }
}
