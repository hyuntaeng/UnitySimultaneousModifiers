using UnityEngine;
using System.Collections;
using System;


public class EffectManager<V>
{
    private Stack effects;
    public Stack CurrentEffects //public getter for the effects stack
    {
        get
        {
            return effects;
        }
    }

    public event EventHandler OnEffectRemoved; //Attach to this event if you don't want 

    public EffectManager(Effect<V> baseEffect)
    {
        if (baseEffect is TemporaryEffect<V>)
        {
            throw new ArgumentException("baseEffect must be a permanent effect NOT a subclass of TemporaryEffect", "baseEffect");
        }
        effects = new Stack();
        AddEffect(baseEffect); //set the base value for the effect
    }

    public void AddEffect(Effect<V> effect) //add an effect and fire its OnBeginTweenIn event
    {
        effects.Push(effect);
        effect.BeginTweenIn();
    }

    private TemporaryEffect<V> PruneEffects() //remove all inactive effects from the top of the stack, returning the last removed effect
    {
        bool madeChange;
        TemporaryEffect<V> lastPopped = null;
        do
        {
            madeChange = false;
            TemporaryEffect<V> effect = effects.Peek() as TemporaryEffect<V>;
            if (effect != null && !effect.IsActive())
            {
                //Remove the effect
                lastPopped = effect;
                effects.Pop();

                madeChange = true;
            }
        } while (madeChange == true);
        return lastPopped;
    }

    //Call this once per frame - prunes effect stack and fires OnEffectRemoved when an effect is made inactive
    public void Update()
    {
        TemporaryEffect<V> effect = PruneEffects();
        if (effect != null)
        {
            effect.BeginTweenOut();
            EventHandler handler = OnEffectRemoved;
            if (handler != null)
            {
                handler(this, new TweenEventArgs<V>(effect.Value, effect.TweenOutTime));
            }
        }
    }
}