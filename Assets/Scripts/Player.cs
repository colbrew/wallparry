using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // SuperParry is touch and hold
	public float durationForSuperParry = 1f;

	public static Player Current { get; private set; }
    public bool IsParrying { get => isParrying; }
    public Vector2 SwipeDirection { get => swipeDirection; }
    public bool SuperParry { get => superParry; }

    private Animation anim;

	private bool isParrying = false;
	private Vector2 touchStartPosition;
	private Vector2 touchEndPosition;
	private float touchStartTime;
	private float touchduration;
	private Vector2 swipeDirection;
	private bool superParry = false;

	private void Awake()
	{
		anim = this.GetComponent<Animation>();
		Current = this;
	}

	private void Update()
	{	
        if(Input.touchCount > 0)
        {
			Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
				case TouchPhase.Began:
					touchStartPosition = touch.position;
					break;

				case TouchPhase.Ended:
					touchEndPosition = touch.position;
                    swipeDirection = (touchEndPosition - touchStartPosition).normalized;
					isParrying = true;
					if (Time.time - touchStartTime > durationForSuperParry)
						superParry = true;
					StartCoroutine("Parry");
					break;
            }
        }
	}

    IEnumerator Parry()
    {
		if (!this.anim.IsPlaying("parry"))
		{
			Debug.Log("PARRY");
			this.anim.Play();
		}

		while (this.anim.isPlaying)
        {
			yield return new WaitForEndOfFrame();
        }

		isParrying = false;
	}
}
