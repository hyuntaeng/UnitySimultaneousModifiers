using UnityEngine;
using System.Collections;

public class HealthLossVignetteColour : MonoBehaviour
{

    public Color colour;
    public float tweenInTime;
    public float holdTime;
    public float tweenOutTime;

    private VignetteManager vignette;

    void Awake()
    {
        vignette = GetComponent< VignetteManager>();
    }

    void Start()
    {
        LivesManager.instance.OnLifeLost += LifeLost;
    }

    void LifeLost(int lives)
    {
        TimedEffect<Color> effect = new TimedEffect<Color>(colour)
        {
            TweenInTime = tweenInTime,
            TweenOutTime = tweenOutTime,
            HoldTime = holdTime
        };
        vignette.AddColourChange(effect);
    }

    void OnDestroy()
    {
        LivesManager.instance.OnLifeLost -= LifeLost;
    }
}