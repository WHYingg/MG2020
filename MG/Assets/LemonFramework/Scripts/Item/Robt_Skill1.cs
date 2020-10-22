using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robt_Skill1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player")
        {
            PlayerCtr ctr = other.gameObject.GetComponent<PlayerCtr>();
            if (ctr.playerType==PlayerType.Player2)
            {
                RobotCtr rCtr = other.gameObject.GetComponent<RobotCtr>();
                rCtr.canStrong = true;
            }
        }
    }
}
