using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HeartBeat))]
public class Player : MonoBehaviour
{
    public static Player Current { get; private set; }

    // parrying anim curve settings
    const float CURVEDURATION = .4f; 
    const float CURVEATMAXHEIGHTTIME = .2f;

    public float durationToChargeSuperParry = 2f; // SuperParry is touch and hold
    public float parryMagnitude = 1;
    [Range(1,5)]
    public int numberOfLives = 3;
    public bool paused = false;

    float chargedFlashRate; // set by checking heart rate

    private Animation anim;
    private SpriteRenderer spriteRend;
    private Color startColor;

    private bool isParrying = false;
    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    private float touchStartTime = 0;
    private float touchduration;
    private Vector2 swipeDirection;
    private bool superParry = false;
    private AnimationCurve xCurve;
    private AnimationCurve yCurve;
    private float startAnimTime;
    private float currAnimTime;
    private bool pulsing = false;
    private Camera cam;
    private HeartBeat hb;
    private int startLives;
    private ObjectShake cameraShake;
    private int score = 0;

    // used to broadcast when player parry's walls
    public delegate void ParryAllWallsEvent();
    public static event ParryAllWallsEvent parryAllWallsEvent;

    public enum numWallsParried
    {
        allWalls,
        singleWall
    }

    // Properties
    public bool IsParrying { get => isParrying; }
    public Vector2 SwipeDirection { get => swipeDirection; }
    public bool SuperParry { get => superParry; }
    public bool Pulsing
    {
        get => pulsing;
        private set
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

    public int Score { get => score; set => score = value; }

    private void Awake()
    {
        anim = this.GetComponent<Animation>();
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        startColor = spriteRend.color;
        cam = Camera.main;
        hb = GetComponent<HeartBeat>();
        startLives = numberOfLives;
        cameraShake = Camera.main.GetComponent<ObjectShake>();
        InitAnimCurves();
        Current = this;
    }

    private void OnEnable()
    {
        Challenge.challengeFailed += LoseLife;
    }

    private void OnDisable()
    {
        Challenge.challengeFailed -= LoseLife;
    }

    private void Update()
    {
        if (!paused)
        {
#if UNITY_IOS || UNITY_ANDROID
            CheckForTouchInput();
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
            CheckForMouseInput();
#endif
        }
    }

    void CheckForTouchInput()
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

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (!Pulsing)
                    {
                        _ = CheckForSuperParryReady();
                    }
                    break;

                case TouchPhase.Ended:
                    if (!isParrying)
                    {
                        isParrying = true;
                        touchEndPosition = touch.position;
                        swipeDirection = (touchEndPosition - touchStartPosition).normalized;

                        StartCoroutine("Parry");
                    }
                    break;
            }
        }
    }

    void CheckForMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartTime = Time.time;
        }

        if (Input.GetMouseButton(0))
        {
            if (!Pulsing)
            {
                _ = CheckForSuperParryReady();
            }
        }

        if (Input.GetMouseButtonUp(0) && !isParrying)
        {
            isParrying = true;
            //Same as TouchPhase.Ended, but start is the center of the object
            touchStartPosition = transform.position;
            touchEndPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            swipeDirection = (touchEndPosition - touchStartPosition).normalized;

            StartCoroutine("Parry");
        }
    }

    bool CheckForSuperParryReady()
    {
        if (Time.time - touchStartTime > durationToChargeSuperParry)
        {
            if (!Pulsing)
            {
                Pulsing = true;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator Parry()
    {
        if (CheckForSuperParryReady())
        {
            superParry = true;
            Pulsing = false;
            parryAllWallsEvent?.Invoke();
            cameraShake.Shake(.4f, .5f);
            this.anim.Play();
            while (this.anim.isPlaying)
                yield return new WaitForEndOfFrame();
            superParry = false;
           
        }
        else
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

        isParrying = false;
    }

    IEnumerator ReverseParry()
    {
        Vector3 tempPos;
        do
        {
            currAnimTime -= Time.deltaTime;
            tempPos = transform.position;
            tempPos.x = xCurve.Evaluate(currAnimTime);
            tempPos.y = yCurve.Evaluate(currAnimTime);
            transform.position = tempPos;
            yield return new WaitForEndOfFrame();
        }
        while (currAnimTime > 0);

        isParrying = false;
    }

    // player color pulsing when ready for super parry
    IEnumerator Pulse()
    {
        while (hb.currentTime >= .1f) // so we start the pulsing at the beginning of a heartbeat anim
        {
            yield return new WaitForEndOfFrame();
        }

        float pulsingStart = Time.time;

        while (Pulsing)
        {
            float o = (Mathf.Sin((Time.time - pulsingStart) * chargedFlashRate) + 1) / 2;
            spriteRend.color = Color.Lerp(startColor, new Color(1f, .8f, .2f), o);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // if player collides with a challenge wall, reverse movement to return to center & shake camera
        if (collision.gameObject.tag == "ChallengeWall")
        {
            if (IsParrying && !superParry)
            {
                StopCoroutine("Parry");
                cameraShake.Shake(.2f, .3f);
                collision.gameObject.GetComponentInParent<Challenge>().IveBeenHit(numWallsParried.singleWall);
                StartCoroutine("ReverseParry");
            }
        }
    }

    public void AddPoints(int points)
    {
        score += points;
        ScoreUI.Instance.AddPoints(points);
    }

    // these curves are used for the player movement when player parries
    void InitAnimCurves()
    {
        Keyframe[] keys;

        // setup x curve
        keys = new Keyframe[3];
        keys[0] = new Keyframe(0.0f, 0.0f);
        keys[1] = new Keyframe(CURVEATMAXHEIGHTTIME, 0.0f);
        keys[2] = new Keyframe(CURVEDURATION, 0.0f);
        xCurve = new AnimationCurve(keys);

        // setup y curve
        keys[0] = new Keyframe(0.0f, 0.0f);
        keys[1] = new Keyframe(CURVEATMAXHEIGHTTIME, 0.0f);
        keys[2] = new Keyframe(CURVEDURATION, 0.0f);
        yCurve = new AnimationCurve(keys);
    }

    // sets the middle key to direction player swiped
    void SetAnimCurves()
    {
        xCurve.MoveKey(1, new Keyframe(CURVEATMAXHEIGHTTIME, swipeDirection.x * parryMagnitude));
        yCurve.MoveKey(1, new Keyframe(CURVEATMAXHEIGHTTIME, swipeDirection.y * parryMagnitude));
    }

    public void SetChargedFlashRate(float bpm)
    {
        chargedFlashRate = ((bpm / 60f) * 2 * Mathf.PI) / 2;
    }

    void LoseLife()
    {
        numberOfLives -= 1;
        if (numberOfLives == 0)
            Level.Current.EndGame();
        GetComponent<ObjectShake>().Shake(.2f, .4f); // shake the player
    }

    public void RestartGame()
    {
        transform.position = Vector3.zero;
        paused = false;
        numberOfLives = startLives;
        score = 0;
    }
}
