using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Current { get; private set; }
    public bool IsParrying { get => isParrying; }
    public Vector2 SwipeDirection { get => swipeDirection; }

    private Animation anim;

	private bool isParrying = false;
	private Vector2 touchStartPosition;
	private Vector2 touchEndPosition;
	private Vector2 swipeDirection;

	private void Awake()
	{
		anim = this.GetComponent<Animation>();
		Current = this;
	}

	private void Update()
	{	//isTouching = Input.GetKey(KeyCode.Space) || Input.touchCount > 0 || Input.GetMouseButton(0);
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
