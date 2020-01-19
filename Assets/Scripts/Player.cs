using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    const float CURVEDURATION = .4f;

    // SuperParry is touch and hold
    public float durationForSuperParry = 2f;

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
    private AnimationCurve xCurve;
    private AnimationCurve yCurve;
    private float startAnimTime;
    private float currAnimTime;
    private bool parryStarted = false;

    private bool mouseDown = false;

    private void Awake()
    {
        anim = this.GetComponent<Animation>();
        Current = this;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPosition = touch.position;
                    touchStartTime = Time.time;
                    break;

                case TouchPhase.Ended:
                    if (!isParrying)
                    {
                        isParrying = true;
                        touchEndPosition = touch.position;
                        swipeDirection = (touchEndPosition - touchStartPosition).normalized;


                        if (Time.time - touchStartTime > durationForSuperParry)
                        {
                            superParry = true;
                            Debug.Log("Super Parry!");
                        }
                        else
                        {
                            Debug.Log("Parry!");
                        }

                        StartCoroutine("Parry");
                    }
                    break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            touchStartTime = Time.time;
            mouseDown = true;
        }

        if (Input.GetMouseButtonUp(0) && !isParrying)
        {
            //Same as TouchPhase.Ended, but start is the center of the object
            mouseDown = false;
            touchStartPosition = transform.position;
            touchEndPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            swipeDirection = (touchEndPosition - touchStartPosition).normalized;
            isParrying = true;
            if (Time.time - touchStartTime > durationForSuperParry)
                superParry = true;
            StartCoroutine("Parry");
        }
    }

    IEnumerator Parry()
    {
        if (!superParry)
        {
            SetAnimCurves();
            startAnimTime = Time.time;
            Vector3 tempPos;
            do
            {
                currAnimTime = Time.time - startAnimTime;
                tempPos = transform.position;
                tempPos.x = xCurve.Evaluate(currAnimTime);
                tempPos.y = yCurve.Evaluate(currAnimTime);
                transform.position = tempPos;
                yield return new WaitForEndOfFrame();
            }
            while (currAnimTime < CURVEDURATION);
        }
        else
        {
            this.anim.Play();
            while (this.anim.isPlaying)
                yield return new WaitForEndOfFrame();
        }

        isParrying = false;
        superParry = false;
  
        yield return null;
    }

    void SetAnimCurves()
    {
        Keyframe[] keys;
        keys = new Keyframe[3];
        keys[0] = new Keyframe(0.0f, 0.0f);
        keys[1] = new Keyframe(.2f, swipeDirection.x);
        keys[2] = new Keyframe(CURVEDURATION, 0.0f);
        xCurve = new AnimationCurve(keys);

        // create a curve to move the GameObject and assign to the clip
        keys[0] = new Keyframe(0.0f, 0.0f);
        keys[1] = new Keyframe(.2f, swipeDirection.y);
        keys[2] = new Keyframe(CURVEDURATION, 0.0f);
        yCurve = new AnimationCurve(keys);

        Debug.Log("Anim curves set");
    }
}
