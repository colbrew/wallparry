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


	// for calculating if your swipe was accurate enough to parry
    // an exactly correct swipe would be -1f, we want something between -1f (very hard/impossible) to -.75f (swipe in general right direciton)
	const float SWIPEACCRUACYLIMIT = -.87f; 

	public float duration = 1.0f;
    [Range(0.0f,1.0f)]
	public float hitWindowMin = 0.75f;
    [Range(0.0f, 1.0f)]
	public float hitWindowMax = 0.9f;
	public Difficulty difficulty;
	public float fadeOutTime = 0.25f;
    [Tooltip("Enter Vector for direction Challenge is moving i.e. (1,-1)")]
	public Vector2 moveDirection;

	private Animation anim;
	private float startTime;
	private State currState;

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
					if (hitWindowMin * duration <= timePassed && timePassed <= hitWindowMax * duration)
					{
                        /*Debug.Log("Move Direction: " + moveDirection.normalized +
                        "/nPlayer direction: " +Player.Current.SwipeDirection +
                        "/nDot Product: " + Vector2.Dot(moveDirection.normalized, Player.Current.SwipeDirection));*/
						if (Vector2.Dot(moveDirection.normalized, Player.Current.SwipeDirection) <= SWIPEACCRUACYLIMIT || Player.Current.SuperParry)
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