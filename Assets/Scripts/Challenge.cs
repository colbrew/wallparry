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

	public float startupDelay = 0.5f;
	public float duration = 1.0f;

	public float hitWindowMin = 0.75f;
	public float hitWindowMax = 0.9f;

	private Animation anim;
	private float startTime;

	private State currState;
	

	private IEnumerator Start()
	{
		currState = State.Playing;
		startTime = -1.0f;
		anim = this.GetComponent<Animation>();
		anim.Stop();
		
		foreach (AnimationState state in anim)
		{
			state.speed =  state.length / duration;
		}

		yield return new WaitForSeconds(startupDelay);

		startTime = Time.time;
		anim.Play();

	}

	private void Update()
	{
		if (startTime > 0 && currState == State.Playing)
		{
			float timePassed = Time.time - startTime;

			bool isParrying = Player.Current.IsParrying;

			if(isParrying)
				Debug.Log("PARRY AT: " + timePassed.ToString());

			if (isParrying && hitWindowMin <= timePassed && timePassed <= hitWindowMax )
			{
				
				currState = State.Success;
				foreach (AnimationState state in anim)
				{
					state.speed *= -1.0f;
				}
			}	
		}
	}

}
