using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ROFLimiter : MonoBehaviour {
	// Made this to add a fire rate to shooting.
	// This will probably be discarded at some point.

	// Private Vars
	private float firesPerSecond = 1f;
	private float fpsCounter, interval, fpsDelta;
	
	private bool canFire, inputDelta, inputPrev, everyFrame, active;

	// Get/Set

	/// <summary>Represents rate of fire per SECOND.</summary>
    /// <value>
	/// gets/sets the number of times it fires per SECOND.
	/// <para>min = 0.0166667f, max = 333.3333333f</para>
	/// </value>
	public float FiresPerSecond
	{
		get
		{
			return firesPerSecond;
		}
		set
		{
			this.firesPerSecond = Mathf.Clamp(value, 0.0166667f, 333.3333333f);
			this.interval = CalculateIntervalTime(firesPerSecond);
			this.fpsCounter = this.interval;
		}
	}
    /// <summary>Represents rate of fire per MINUTE.</summary>
    /// <value>
	/// gets/sets the number of times it fires per MINUTE.
	/// <para>min = 1f, max = 20,000f</para>
	/// </value>
    public float FiresPerMinute
	{
		get
		{
			return firesPerSecond * 60f;
		}
		set
		{
			this.firesPerSecond = Mathf.Clamp(value / 60f, 0.0166667f, 333.3333333f);
			this.interval = CalculateIntervalTime(firesPerSecond);
			this.fpsCounter = this.interval;
		}
	}
    /// <summary>This sets the script to fire on everyframe.</summary>
    /// <value>true = Fire every frame, false = Limit fire rate.</value>
    public bool FireEveryFrame
	{
		set
		{
			if(this.everyFrame != value) { this.everyFrame = value; }
		}
	}
	

	// Use this for initialization
	void Start () {
		interval = CalculateIntervalTime(firesPerSecond);
		fpsCounter = interval;
		active = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    /// <summary>
	/// Returns whether or not to fire based on a limited rate of fire.
	/// </summary>
	/// <param name="input">
	/// bool true = attempt to fire, false = do not fire.
	/// </param>
	/// <remarks>
	/// This script will only fire as fast as your frame rate. 
	/// For example, a frames per second of 60 will limit the max fire rate to 60.
	/// </remarks>
	/// <example> 
	/// How to use:
	/// <code>
	/// if (weap1.CanFire(Input.GetMouseButton(0)))
	/// {
	///     Shoot();
	/// }
	/// </code>
	/// </example>
	public bool CanFire(bool input)
	{
		if(active == false) { return false; }
		if(everyFrame == true) { return true; }
		if (handleInput(input))
		{
			fpsCounter = interval;
		}
		if (input)
		{
			fpsCounter += Time.deltaTime;
			if (fpsCounter >= interval)
			{
				fpsCounter = 0f;
				return true;
			}
		}
		return false;
	}
	

    /// <summary>Sets this limiter to active state and allows for firing.</summary>
	public void Enable()
	{
		active = true;
	}
    /// <summary>Sets this limiter to inactive state and does not allow firing.</summary>
	public void Disable()
	{
		active = false;
	}



	private float CalculateIntervalTime(float fps)
	{
		return 1f / fps;
	}
	private bool handleInput(bool input)
	{
		if (input != inputDelta)
		{
			if (input == true && inputDelta == false)
			{
				inputDelta = input;
				return true;
			}
		}
		return false;
	}
}
