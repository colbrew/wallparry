using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Current { get; private set; }

	private Animation anim;

	private void Awake()
	{
		anim = this.GetComponent<Animation>();
		Current = this;
	}

	private void Update()
	{
		wasTouching = isTouching;
		isTouching = Input.GetKey(KeyCode.Space) || Input.touchCount > 0 || Input.GetMouseButton(0);
		if (!wasTouching && isTouching && !this.anim.IsPlaying("parry"))
		{
			Debug.Log("PARRY");
			this.anim.Play();
		}
	}

	private bool wasTouching = false;
	private bool isTouching = false;

	public bool IsParrying { get { return !wasTouching && isTouching; } }
}
