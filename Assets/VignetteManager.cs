using UnityEngine;
using System.Collections;
using System;


public class VignetteManager : MonoBehaviour
{

    public static VignetteManager instance;

    public EffectManager<Color> colourManager;

    public Color baseColour;

    void Awake() //enforce Monobehaviour Singleton pattern
    {
        if (instance == null)
        {
            instance = this;
            //Set initial vignette colour
            colourManager = new EffectManager<Color>(AddEventsToColourChange(new Effect<Color>(baseColour)));
            colourManager.OnEffectRemoved += TweenBack;
        }
        else
        {
            Destroy(this);
        }
    }

    public void AddColourChange(Effect<Color> change)
    {
        change = AddEventsToColourChange(change);
        colourManager.AddEffect(change);
    }

    private Effect<V> AddEventsToColourChange<V>(Effect<V> change)
    {
        change.OnTweenInBegin += Tween;
        return change;
    }

    void TweenBack(object sender, EventArgs a)
    {
        TweenEventArgs<Color> args = (TweenEventArgs<Color>)a;
        Effect<Color> topEffect = (Effect<Color>)colourManager.CurrentEffects.Peek();
        TweenTo(args.TweenTime, topEffect.Value);
    }

    void TweenTo(float tweenTime, Color destinationValue)
    {
        //TweenColor.Begin(gameObject, tweenTime, destinationValue);
        iTween.ColorTo(gameObject, destinationValue, tweenTime);

    }

    void Tween(object sender, EventArgs a)
    {
        TweenEventArgs<Color> args = (TweenEventArgs<Color>)a;
        TweenTo(args.TweenTime, args.DestinationValue);
    }

    void Update() //Unity runs this once per frame
    {
        colourManager.Update();
    }
}