using UnityEngine;
using System.Collections;
using System;




public class Effect<V>
{
    public float TweenInTime = 0;

    public event EventHandler OnTweenInBegin;  //invoked when the effect begins tweening in

    public void BeginTweenIn()
    {
        EventHandler handler = OnTweenInBegin;
        if (handler != null)
        {
            handler(this, new TweenEventArgs<V>(Value, TweenInTime));
        }
    }

    public Effect()
    {
    }

    public Effect(V value)
    {
        this.Value = value;
    }

    public V Value; //The final value (after tweening) to set the property we're affecting to.
}