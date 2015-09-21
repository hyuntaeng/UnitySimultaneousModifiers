using UnityEngine;
using System.Collections;
using System;

public abstract class TemporaryEffect<V> : Effect <V>
{
    public float TweenOutTime = 0;

    public event EventHandler OnTweenOutBegin; //Fires when the effect begins to tween out

    public TemporaryEffect() : base()
    {
        
    }

    public TemporaryEffect(V value) : base(value)
    {

    }

    public void BeginTweenOut()
    {
        EventHandler handler = OnTweenOutBegin;
        if (handler != null)
        {
            handler(this, new TweenEventArgs<V>(Value, TweenOutTime));
        }
    }

    public abstract bool IsActive(); //Should we keep this effect alive?
}