using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPanel : PanelBase
{
    private Image test;

    // Start is called before the first frame update
    void Start()
    {
        test = GetControl<Image>("TestImage");
        test.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
