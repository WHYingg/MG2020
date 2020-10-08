using System;
public enum PlotName
{
    Sample,
    
}

public enum PlotState
{
    Unactive,
    Listening,
    Executed,
}

public struct PlotEvent { 
    public PlotState state;
    public Delegate listener;
    public Delegate callback;
    public PlotEvent(PlotState state,Delegate listener, Delegate callback)
    {
        this.state = state;
        this.listener = listener;
        this.callback = callback;
    }
}