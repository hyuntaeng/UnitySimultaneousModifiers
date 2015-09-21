using UnityEngine;
using System.Collections;
using System;




public class LivesManager : MonoBehaviour
{
    public delegate void LostHandler(int a);

    public static LivesManager instance;

    public LostHandler OnLifeLost;
    
    void Awake() //enforce Monobehaviour Singleton pattern
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void DecreaseLife()
    {
        OnLifeLost(0);
    }
}