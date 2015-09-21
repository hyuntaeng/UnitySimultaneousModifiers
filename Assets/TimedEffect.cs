using UnityEngine;
using System.Collections;
using System;

public class TimedEffect<V> : TemporaryEffect<V>
{
    public float HoldTime = 0;

    private float startTime; //The time since the level loaded that this object was created

    public TimedEffect() : base()
    {
        startTime = Time.timeSinceLevelLoad;
    }

    public TimedEffect(V value) : base(value)
    {
        startTime = Time.timeSinceLevelLoad;
    }

    public override bool IsActive() //Has less time passed since this effect was created than TweenInTime + HoldTime?
    {
        return (Time.timeSinceLevelLoad - startTime) < (TweenInTime + HoldTime);
    }
}