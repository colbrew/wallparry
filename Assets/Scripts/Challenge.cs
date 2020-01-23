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

    private Animation anim;
    private float startTime = 0;
    private State currState;
    bool parryProcessed = false;
    private float timePassed;

    public float TimePassed { get => Time.time - startTime; }

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

    private void OnEnable()
    {
        Player.parryEvent += CheckForHit;
    }

    private void OnDisable()
    {
        Player.parryEvent -= CheckForHit;
    }

    private void Update()
    {
        if (startTime > 0 && currState == State.Playing)
        {

            if (TimePassed >= duration)
            {
               currState = State.Failed;
            }
        }
    }

    void CheckForHit()
    {
        if (hitWindowMin * duration <= TimePassed && TimePassed <= hitWindowMax * duration)
        {
            Debug.Log("Move Direction: " + moveDirection.normalized +
            "Player direction: " + Player.Current.SwipeDirection +
            "Dot Product: " + Vector2.Dot(moveDirection.normalized, Player.Current.SwipeDirection));
            // check if parry was on target
            if (Vector2.Dot(moveDirection.normalized, Player.Current.SwipeDirection) <= Player.Current.SWIPEACCRUACYLIMIT || Player.Current.SuperParry)
            {
                foreach (AnimationState state in anim)
                {
                    state.speed *= -1.0f;
                }
                Debug.Log("Parry hit target");
            }
            else
            {
                parryProcessed = false; //give them a chance to parry again
                Debug.Log("Parry was off target!");
            }
        }
    }
  
    void ChallengeFailed()
    {

    }
}