using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboEffect<V> : TemporaryEffect<V>
{
    public List<Effect<V>> Effects; //All of the encompassing effects (Value is unused)

    public enum Operation
    {
        AND,
        OR
    }

    public Operation Operator;

    public ComboEffect() : base()
    {
        Effects = new List<Effect<V>>();
    }

    public ComboEffect(V value) : base(value)
    {
        Effects = new List<Effect<V>>();
    }

    public override bool IsActive()
    {
        switch (Operator)
        {
            case Operation.AND: //Return true if all effects are active
                foreach (TemporaryEffect<V> effect in Effects)
                {
                    if (!effect.IsActive())
                    {
                        return false;
                    }
                }
                return true;
            case Operation.OR: //Return true if at least one effect is active
                foreach (TemporaryEffect<V> effect in Effects)
                {
                    if (effect.IsActive())
                    {
                        return true;
                    }
                }
                return false;
        }
        return false;
    }
}