using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Challenge : MonoBehaviour
{

	public enum State
	{
		Playing,
		Success,
		Failed
	}

	public enum Difficulty
	{
		Easy,
		Medium,
		Hard,
	}


	public float duration = 1.0f;

	public float hitWindowMin = 0.75f;
	public float hitWindowMax = 0.9f;

	public Difficulty difficulty;

	public float fadeOutTime = 0.25f;


	private Animation anim;
	private float startTime;

	private State currState;

	private void Awake()
	{
		currState = State.Playing;
		anim = this.GetComponent<Animation>();		
		foreach (AnimationState state in anim)
		{
			state.speed =  state.length / duration;
		}

		startTime = Time.time;
		anim.Play();

		GameObject.Destroy(this.gameObject, duration + fadeOutTime);
	}

	private void Update()
	{
		if (startTime > 0 && currState == State.Playing)
		{
			float timePassed = Time.time - startTime;

			if (timePassed >= duration)
			{
				currState = State.Failed;
			}
			else
			{
				bool isParrying = Player.Current.IsParrying;

				if (isParrying)
					Debug.Log("PARRY AT: " + timePassed.ToString());

				if (isParrying && hitWindowMin <= timePassed && timePassed <= hitWindowMax)
				{

					currState = State.Success;
					foreach (AnimationState state in anim)
					{
						state.speed *= -1.0f;
					}
				}
			}
		}

		if (currState == State.Failed)
		{

		}
	}

}
