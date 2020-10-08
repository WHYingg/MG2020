using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlotCenter : MonoBehaviour
{
    Dictionary<PlotName, PlotEvent> plots;
    // Start is called before the first frame update

    void PlotRegister()
    {

        //plots.Add(PlotName.Sample, new PlotEvent(PlotState.Unactive,new CallBack(PlotListener.Listener), new CallBack(PlotCallBack.Callback)));
    }


    void Awake()
    {
        PlotRegister();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var plot in plots)
        {
            PlotEvent plotEvent = plot.Value;
            if(plotEvent.state == PlotState.Listening)
            {
                ((CallBack)plotEvent.callback)();
                plotEvent.state = PlotState.Executed;
            }
        }
    }
}
