using UnityEngine;
using System.Collections;
using System;

public class TweenEventArgs<V> : EventArgs
{
    public float TweenTime = 0f;
    public V DestinationValue;

    public TweenEventArgs(V destinationValue) : base()
    {
        this.DestinationValue = destinationValue;
    }

    public TweenEventArgs(V destinationValue, float tweenTime) : base()
    {
        this.DestinationValue = destinationValue;
        this.TweenTime = tweenTime;
    }
}