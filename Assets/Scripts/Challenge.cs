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
    [Range(0.0f, 1.0f)]
    public float hitWindowMin = 0.75f;
    [Range(0.0f, 1.0f)]
    public float hitWindowMax = 0.9f;
    public Difficulty difficulty;
    public float fadeOutTime = 0.25f;
    [Tooltip("Enter Vector for direction Challenge is moving i.e. (1,-1)")]
    public Vector2 moveDirection;
    public Color failColor = Color.red;
    public int pointsValue = 100;

    private Animation anim;
    private float startTime = 0;
    public State currState;
    private float timePassed;
    private SpriteRenderer sr;

    public float TimePassed { get => Time.time - startTime; }

    public SpriteAnimation spriteAnimation;

    public delegate void ChallengeFailed();
    public static event ChallengeFailed challengeFailed;

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
        sr = GetComponentInChildren<SpriteRenderer>();

        GameObject.Destroy(this.gameObject, duration + fadeOutTime);
    }

    private void OnEnable()
    {
        Player.parryAllWallsEvent += ParryAllWalls;
    }

    private void OnDisable()
    {
        Player.parryAllWallsEvent -= ParryAllWalls;
    }

    private void Update()
    {
        if (startTime > 0)
        {
            if (TimePassed >= duration && currState == State.Playing)
            {
                currState = State.Failed;
                challengeFailed?.Invoke();
                StartCoroutine(ChallengeDie());
            }
        }
    }

    public void ParryAllWalls()
    {
            IveBeenHit(Player.numWallsParried.allWalls);
    }

    // returns number of points from the hit
    public void IveBeenHit(Player.numWallsParried numWallsParried)
    {
        if (currState == State.Success || currState == State.Failed)
            return;

        currState = State.Success;
        if(spriteAnimation != null) spriteAnimation.PlayAnimation(0, 1/24f);
        foreach (AnimationState state in anim)
        {
            state.speed *= -1.0f;
        }

        Debug.Log("Parry hit target");

        int points = 0;
        if (numWallsParried == Player.numWallsParried.singleWall)
        {
            // points are currently affected by the accuracy of the hit
            points = ((int)(pointsValue * AccuracyOfHit())) * 10;
        }
        else if (numWallsParried == Player.numWallsParried.allWalls)
        {
            points = pointsValue * 10;
        }

        Player.Current.AddPoints(points);
    }

    float AccuracyOfHit()
    {
        Debug.Log("Move Direction: " + moveDirection.normalized +
                  "Player direction: " + Player.Current.SwipeDirection +
                  "Dot Product: " + Vector2.Dot(moveDirection.normalized, Player.Current.SwipeDirection));

        float x = Mathf.Max((Vector2.Dot(moveDirection.normalized, Player.Current.SwipeDirection) * -1), 0);

        x *= x * x * x; // this affects how quickly the points go down the more you are off target

        return x;
    }

    // makes wall change color and destroy itself when player fails to parry it away in time
    IEnumerator ChallengeDie()
    {
        float u = 0;
        Color startColor = sr.color;
        while (u < 1)
        {
            sr.color = Color.Lerp(startColor, failColor, u);
            u += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}