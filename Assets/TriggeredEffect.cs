using UnityEngine;
using System.Collections;

public class TriggeredEffect <V>: TemporaryEffect<V>
{
    public bool IsEnabled = true; //Is the effect still active?

    public TriggeredEffect() : base()
    {

    }

    public TriggeredEffect(V value) : base(value)
    {

    }

    public override bool IsActive() //Is IsEnabled still true?
    {
        return IsEnabled;
    }
}