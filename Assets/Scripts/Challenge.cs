using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Challenge : MonoBehaviour
{

	public enum State
	{
		Playing,
		Parrying,
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

	private Vector2 moveDirection = new Vector2(-1, -1);

	private void Awake()
	{
		currState = State.Playing;
		anim = this.GetComponent<Animation>();
		foreach (AnimationState state in anim)
		{
			state.speed = state.length / duration;
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
				{
					currState = State.Parrying;
					if (hitWindowMin <= timePassed && timePassed <= hitWindowMax)
					{
						if (Vector2.Dot(moveDirection.normalized, Player.Current.SwipeDirection) < -.94f)
						{
							currState = State.Success;
							foreach (AnimationState state in anim)
							{
								state.speed *= -1.0f;
							}
							Debug.Log("PARRY AT: " + timePassed.ToString());
						}
						else
						{
							Debug.Log("Parry was off target!");
							currState = State.Failed;
						}
					}
					else
					{
						currState = State.Failed;
					}
				}
			}
		}
	}
}