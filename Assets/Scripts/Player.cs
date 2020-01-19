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
    private bool Pulsing {
        get => pulsing;
        set
        {
            pulsing = value;
            if (pulsing)
            {
                StartCoroutine("Pulse");
            }
            else
            {
                spriteRend.color = startColor;
            }
        }
    }


    private Animation anim;
    private SpriteRenderer spriteRend;
    private Color startColor;

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
    private bool pulsing = false;

    private bool mouseDown = false;

    private void Awake()
    {
        anim = this.GetComponent<Animation>();
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        startColor = spriteRend.color;
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

        if(Input.GetMouseButton(0))
        {
            if(Time.time - touchStartTime > durationForSuperParry && !Pulsing)
            {
                Pulsing = true;
            }
        }

        if (Input.GetMouseButtonUp(0) && !isParrying)
        {
            isParrying = true;
            //Same as TouchPhase.Ended, but start is the center of the object
            mouseDown = false;
            touchStartPosition = transform.position;
            touchEndPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            swipeDirection = (touchEndPosition - touchStartPosition).normalized;
            if (Time.time - touchStartTime > durationForSuperParry)
            {
                Pulsing = false;
                superParry = true;
            }
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

    IEnumerator Pulse()
    {
        float pulsingStart = Time.time;
        Debug.Log("Ready for super parry");
        while (Pulsing)
        {
            float o = Mathf.PingPong(Time.time - pulsingStart, 1);
            spriteRend.color = Color.Lerp(startColor, new Color(1f, .8f, .2f), o);
            yield return new WaitForEndOfFrame();
        }

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
