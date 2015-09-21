# UnitySimultaneousModifiers
유니티에서 적용되는 효과를 동시수정.


http://www.udellgames.com/posts/effect-manager/


Effect Manager – How to manage simultaneous modifiers
7 Replies
HyperGauntlet slow motion vignette using the Effect ManagerIn Hyper Gauntlet, I have recently implemented a vignette system for the edges of the screen. Vignettes are coloured based on the changing game state, with a slow blue fade in and out during slow motion and a red flash when you hit an obstacle. Originally I tweened between colours as needed, but I soon met errors in common scenarios when the vignette colour must tween back to a previous effect colour. As an example, imagine an obstacle is approaching fast. I hold space to engage the slow motion, and a blue vignette appears around the screen. Unfortunately, I cannot move out of the way in time and I hit the obstacle – causing the vignette to fade quickly to red. In a basic system, the vignette would then fade back to a neutral state, instead of returning to the blue to signify that slow motion was still engaged, and this confuses players. Today I present a simple solution tailored for Unity systems that applies to any code base. It’s not just useful for vignettes either, I intend to also apply this system to slow motion (to apply power up-induced slow motion even when the player lets go of space for manual slow motion), and this could also find a use in stacking effect spells in RPGs.

Effect

Let’s start with the simplest class. It’s an effect. It doesn’t really do much at all, you’re meant to extend from it or any of its derived classes with your own, more specialised classes. Effect acts as a catchall for all effects so that they can all be processed. It does support events for tweening in though, as all effects can be gradually applied.


public class Effect
{
	public float TweenInTime =0;

	public event EventHandler OnTweenInBegin;  //invoked when the effect begins tweening in

	public void BeginTweenIn()
	{
		EventHandler handler = OnTweenInBegin;
		if(handler!= null)
		{
			handler(this,new TweenEventArgs(Value,TweenInTime));
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
1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
public class Effect
{
	public float TweenInTime =0;
 
	public event EventHandler OnTweenInBegin;  //invoked when the effect begins tweening in
 
	public void BeginTweenIn()
	{
		EventHandler handler = OnTweenInBegin;
		if(handler!= null)
		{
			handler(this,new TweenEventArgs(Value,TweenInTime));
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
TweenEventArgs

You may have noticed that Effect fires an event, OnTweenInBegin which uses a custom EventArgs implementation called TweenEventArgs. All we hold in TweenEventArgs are the destination colour of the effect being processed and the tween time, which we get straight from the effect itself. The source for this looks like this.


public class TweenEventArgs : EventArgs
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
1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
public class TweenEventArgs : EventArgs
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
Temporary Effect

This is only a small extension of the Effect Base class. The Temporary Effect will make up most of the effects applied. These effects both tween in and out and have events to handle this, as well as an abstract function IsActive() which the Effect Manager will use to decide whether to keep or discard the effect.


public abstract class TemporaryEffect : Effect
{
	public float TweenOutTime =0;

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
		if(handler!= null)
		{
			handler(this,new TweenEventArgs(Value,TweenOutTime));
		}
	}

	public abstract bool IsActive(); //Should we keep this effect alive?
}
1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
27
public abstract class TemporaryEffect : Effect
{
	public float TweenOutTime =0;
 
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
		if(handler!= null)
		{
			handler(this,new TweenEventArgs(Value,TweenOutTime));
		}
	}
 
	public abstract bool IsActive(); //Should we keep this effect alive?
}
Timed Effect

Finally, we get to the meat of the effects system. The Timed Effect applies itself to an object for a certain amount of time and then the Effect Manager removes it. By its nature it’s a temporary effect, and thus has both tweening in and out, as well as a new field for how long after the tween in completes before the Tween out should start.


public class TimedEffect : TemporaryEffect
{
	public float HoldTime =0;

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
1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
public class TimedEffect : TemporaryEffect
{
	public float HoldTime =0;
 
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
Here, Time.timeSinceLevelLoad is a Unity utility field which holds how many seconds have passed since the level completed loading.

Triggered Effect

The Triggered Effect is a temporary effect that begins enabled until it is disabled externally through a function call. This kind of effect is useful for player-controlled effects, such as those dependent on a button being held down, or an effect dependent on more complicated code than simple timing.


public class TriggeredEffect : TemporaryEffect
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
1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
public class TriggeredEffect : TemporaryEffect
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
Combo Effect

The combo effect allows you to combine effects as needed. For example, if you want to have apply an effect for as long as a button is held down, but at the same time want to restrict how long the button can be held down for (a good example is how Hyper Gauntlet handles the slow motion button). Slightly more in-depth than the previous effects, the Combo Effect allows you to specify a combination operation based on Boolean AND or Boolean OR.


public class ComboEffect : TemporaryEffect
{
	public List Effects; //All of the encompassing effects (Value is unused)

	public enum Operation
	{
		AND,
		OR
	}

	public Operation Operator;

	public ComboEffect() : base()
	{
		Effects = new List();
	}

	public ComboEffect(V value) : base(value)
	{
		Effects = new List();
	}

	public override bool IsActive() 
	{
		switch(Operator)
		{
			case Operation.AND:	//Return true if all effects are active
				foreach(TemporaryEffect effect in Effects)
				{
					if(!effect.IsActive())
					{
						return false;
					}
				}
				return true;
			case Operation.OR: //Return true if at least one effect is active
				foreach(TemporaryEffect effect in Effects)
				{
					if(effect.IsActive())
					{
						return true;
					}
				}
				return false;
		}
		return false;
	}
}
1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
27
28
29
30
31
32
33
34
35
36
37
38
39
40
41
42
43
44
45
46
47
48
public class ComboEffect : TemporaryEffect
{
	public List Effects; //All of the encompassing effects (Value is unused)
 
	public enum Operation
	{
		AND,
		OR
	}
 
	public Operation Operator;
 
	public ComboEffect() : base()
	{
		Effects = new List();
	}
 
	public ComboEffect(V value) : base(value)
	{
		Effects = new List();
	}
 
	public override bool IsActive() 
	{
		switch(Operator)
		{
			case Operation.AND:	//Return true if all effects are active
				foreach(TemporaryEffect effect in Effects)
				{
					if(!effect.IsActive())
					{
						return false;
					}
				}
				return true;
			case Operation.OR: //Return true if at least one effect is active
				foreach(TemporaryEffect effect in Effects)
				{
					if(effect.IsActive())
					{
						return true;
					}
				}
				return false;
		}
		return false;
	}
}
Effect Manager

The Effect Manager handles all active effects for a single property. You might have one for max. health in an RPG, or in the case of Hyper Gauntlet, for the speed of time. An Effect Manager operates a Stack of active Effect objects. When you add an effect, they’re put on the top of the stack, and when the top of the stack becomes inactive, the Effect Manager removes it. This manager supports both cumulative effects (by attaching to the OnBeginTweenIn and OnBeginTweenOut events of the Effects) and effects where the top of the Stack overrides all Effects beneath it (by attaching to the OnEffectRemoved event in the manager and checking the currently active effects stack).


public class EffectManager
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

	public EffectManager(Effect baseEffect)
	{
		if(baseEffect is TemporaryEffect)
		{
			throw new ArgumentException("baseEffect must be a permanent effect NOT a subclass of TemporaryEffect","baseEffect");
		}
		effects = new Stack();
		AddEffect(baseEffect); //set the base value for the effect
	}

	public void AddEffect(Effect effect) //add an effect and fire its OnBeginTweenIn event
	{
		effects.Push(effect);
		effect.BeginTweenIn();
	}

	private TemporaryEffect PruneEffects() //remove all inactive effects from the top of the stac, returning the last removed effect
	{
		bool madeChange;
		TemporaryEffect lastPopped = null;
		do
		{
			madeChange = false;
			TemporaryEffect effect = effects.Peek() as TemporaryEffect;
			if(effect != null && !effect.IsActive())
			{
				//Remove the effect
				lastPopped = effect;
				effects.Pop();

				madeChange = true;
			}
		} while(madeChange == true);
		return lastPopped;
	}

	//Call this once per frame - prunes effect stack and fires OnEffectRemoved when an effect is made inactive
	public void Update()
	{
		TemporaryEffect effect = PruneEffects();
		if(effect != null)
		{
			effect.BeginTweenOut();
			EventHandler handler = OnEffectRemoved;
			if(handler!= null)
			{
				handler(this,new TweenEventArgs(effect.Value,effect.TweenOutTime));
			}
		}
	}
}
1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
27
28
29
30
31
32
33
34
35
36
37
38
39
40
41
42
43
44
45
46
47
48
49
50
51
52
53
54
55
56
57
58
59
60
61
62
63
64
public class EffectManager
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
 
	public EffectManager(Effect baseEffect)
	{
		if(baseEffect is TemporaryEffect)
		{
			throw new ArgumentException("baseEffect must be a permanent effect NOT a subclass of TemporaryEffect","baseEffect");
		}
		effects = new Stack();
		AddEffect(baseEffect); //set the base value for the effect
	}
 
	public void AddEffect(Effect effect) //add an effect and fire its OnBeginTweenIn event
	{
		effects.Push(effect);
		effect.BeginTweenIn();
	}
 
	private TemporaryEffect PruneEffects() //remove all inactive effects from the top of the stac, returning the last removed effect
	{
		bool madeChange;
		TemporaryEffect lastPopped = null;
		do
		{
			madeChange = false;
			TemporaryEffect effect = effects.Peek() as TemporaryEffect;
			if(effect != null && !effect.IsActive())
			{
				//Remove the effect
				lastPopped = effect;
				effects.Pop();
 
				madeChange = true;
			}
		} while(madeChange == true);
		return lastPopped;
	}
 
	//Call this once per frame - prunes effect stack and fires OnEffectRemoved when an effect is made inactive
	public void Update()
	{
		TemporaryEffect effect = PruneEffects();
		if(effect != null)
		{
			effect.BeginTweenOut();
			EventHandler handler = OnEffectRemoved;
			if(handler!= null)
			{
				handler(this,new TweenEventArgs(effect.Value,effect.TweenOutTime));
			}
		}
	}
}
In action

Here’s an example of the Effect Manager being used to manage the vignette system in Hyper Gauntlet. I define the Vignette Manager class as a subclass of Unity’s MonoBehaviour class, and manage colour tweening on the OnBeginTweenIn and OnEffectRemoved events. I don’t handle the OnBeginTweenOut event because I am reverting the colour of the vignette to the top colour on the stack, not removing the effects of the colour (you can think of it as the difference between setting an integer from 6 to 4 or removing a +2 effect on the integer).


public class VignetteManager : MonoBehaviour {

	public static VignetteManager instance;

	public EffectManager colourManager;

	public Color baseColour;

	void Awake() //enforce Monobehaviour Singleton pattern
	{
		if(instance == null)
		{
			instance = this;
			//Set initial vignette colour
			colourManager = new EffectManager(AddEventsToColourChange(new Effect(baseColour)));
			colourManager.OnEffectRemoved+=TweenBack;
		}
		else
		{
			Destroy(this);
		}
	}

	public void AddColourChange(Effect change)
	{
		change = AddEventsToColourChange(change);
		colourManager.AddEffect(change);	
	}

	private Effect AddEventsToColourChange(Effect change)
	{
		change.OnTweenInBegin+=Tween;
		return change;
	}

	void TweenBack(object sender, TweenEventArgs args)
	{
		Effect topEffect = colourManager.CurrentEffects.Peek();
		TweenTo(args.TweenTime,topEffect.Value);
	}

	void TweenTo(float tweenTime, Color destinationValue)
	{
		TweenColor.Begin(gameObject, tweenTime, destinationValue);
	}

	void Tween(object sender, TweenEventArgs args)
	{
		TweenTo(args.TweenTime,args.DestinationValue);
	}

	void Update() //Unity runs this once per frame
	{
		colourManager.Update();
	}
}
1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
27
28
29
30
31
32
33
34
35
36
37
38
39
40
41
42
43
44
45
46
47
48
49
50
51
52
53
54
55
56
public class VignetteManager : MonoBehaviour {
 
	public static VignetteManager instance;
 
	public EffectManager colourManager;
 
	public Color baseColour;
 
	void Awake() //enforce Monobehaviour Singleton pattern
	{
		if(instance == null)
		{
			instance = this;
			//Set initial vignette colour
			colourManager = new EffectManager(AddEventsToColourChange(new Effect(baseColour)));
			colourManager.OnEffectRemoved+=TweenBack;
		}
		else
		{
			Destroy(this);
		}
	}
 
	public void AddColourChange(Effect change)
	{
		change = AddEventsToColourChange(change);
		colourManager.AddEffect(change);	
	}
 
	private Effect AddEventsToColourChange(Effect change)
	{
		change.OnTweenInBegin+=Tween;
		return change;
	}
 
	void TweenBack(object sender, TweenEventArgs args)
	{
		Effect topEffect = colourManager.CurrentEffects.Peek();
		TweenTo(args.TweenTime,topEffect.Value);
	}
 
	void TweenTo(float tweenTime, Color destinationValue)
	{
		TweenColor.Begin(gameObject, tweenTime, destinationValue);
	}
 
	void Tween(object sender, TweenEventArgs args)
	{
		TweenTo(args.TweenTime,args.DestinationValue);
	}
 
	void Update() //Unity runs this once per frame
	{
		colourManager.Update();
	}
}
Then attached to the same object as the VignetteManager I have a lot of classes that look like the following.


public class HealthLossVignetteColour : MonoBehaviour {

	public Color colour;
	public float tweenInTime;
	public float holdTime;
	public float tweenOutTime;

	private VignetteManager vignette;

	void Awake()
	{
		vignette = GetComponent();
	}

	void Start()
	{
		LivesManager.Instance.OnLifeLost+= LifeLost;
	}

	void LifeLost(int lives)
	{
		TimedEffect effect = new TimedEffect(colour)
		{
			TweenInTime=tweenInTime,
			TweenOutTime = tweenOutTime,
			HoldTime = holdTime
		};
		vignette.AddColourChange(effect);
	}

	void OnDestroy()
	{
		LivesManager.Instance.OnLifeLost-= LifeLost;
	}
}
1
2
3
4
5
6
7
8
9
10
11
12
13
14
15
16
17
18
19
20
21
22
23
24
25
26
27
28
29
30
31
32
33
34
35
public class HealthLossVignetteColour : MonoBehaviour {
 
	public Color colour;
	public float tweenInTime;
	public float holdTime;
	public float tweenOutTime;
 
	private VignetteManager vignette;
 
	void Awake()
	{
		vignette = GetComponent();
	}
 
	void Start()
	{
		LivesManager.Instance.OnLifeLost+= LifeLost;
	}
 
	void LifeLost(int lives)
	{
		TimedEffect effect = new TimedEffect(colour)
		{
			TweenInTime=tweenInTime,
			TweenOutTime = tweenOutTime,
			HoldTime = holdTime
		};
		vignette.AddColourChange(effect);
	}
 
	void OnDestroy()
	{
		LivesManager.Instance.OnLifeLost-= LifeLost;
	}
}
These attach to various in-game events, such as hitting an obstacle in this example, and add Effect objects on the vignette manager. As you can see, it’s incredibly simple to add new vignette effects to this system.

As you can see this is a powerful system for managing a common problem in games. You’re free to use this code however you like, but I make no guarantee that it will work (although it does for me!). On your head be it.

Share this:
Facebook3
Twitter
Reddit
Google
Tumblr
Email
More
Related Posts:
Vignetting and communicating with the player without distracting them In the past couple of days I’ve added a vignette...
Hyper Gauntlet updated to v1.0.1 If you already own Hyper Gauntlet, please re-download it from...
2013: A year in postmortem, part 1- Hyper Gauntlet It’s been a big year for me. Udell Games did...
A few helpful classes for text generation I’ve been in Greece for a conference on biomechanics for...
This entry was posted in Development Journal, Hyper Gauntlet and tagged classes, Code, dev journal, effects, object oriented programming, progress, Tech, unity3d, vignette on October 31, 2013.
Post navigation← PixelShit Wednesday Numero UnoPixelShit Wednesday First Blood Part 2 →
7 thoughts on “Effect Manager – How to manage simultaneous modifiers”


Simplegamings
November 1, 2013 at 12:58 am
Looks like a nice clean way to get it done. This post was well done, mind if I talk about it on simplegamings.com? I thought it was something worth sharing.


(People often like example code)
1
(People often like example code)
Reply ↓