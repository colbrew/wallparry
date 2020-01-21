using System.Collections;
using UnityEngine;

public class UIJuicer : MonoBehaviour
{
    public bool juicePosition;
    [Tooltip("Amount to offset end position to set start position - often moving item off screen to start.")]
    public Vector3 offscreenPosOffset;
    public bool juiceScale;
    public Vector3 startScale;
    public AnimationCurve lerpCurve;
    public float enterDelay = 0;
    public float exitDelay = 0;
    public float duration = 1;
    [Tooltip("Start offscreen")]
    public bool startOffscreen = false;
    [Tooltip("Start the movement as soon as object is enabled.")]
    public bool automaticJuiceOnEnable = true;
    [Tooltip("Do you want idle motion on this object after it finishes entry?")]
    public bool idleAfterEntrance;
    public bool deactivateAfterExit;

    public Vector3 endScale;
    public Vector3 startPos;
    public Vector3 endPos;
    private IdleMotion im;

    public Vector3 EndPos
    {
        get
        {
            return endPos;
        }

        set
        {
            endPos = value;
        }
    }

    private void Awake()
    {
        im = gameObject.GetComponent<IdleMotion>();
        if (lerpCurve.keys.Length < 2)
            Debug.LogError("Must set lerpCurve in Inspector of " + gameObject.name + " for UIJuicer to work.");
    }

    private void Start()
    {
        if (juicePosition)
        {
            EndPos = transform.localPosition;
            startPos = EndPos + offscreenPosOffset;
            if (automaticJuiceOnEnable || startOffscreen)
                transform.localPosition = startPos;
        }

        if (juiceScale)
        {
            endScale = transform.localScale;
            if (automaticJuiceOnEnable || startOffscreen)
                transform.localScale = startScale;
        }

        if (automaticJuiceOnEnable)
            StartCoroutine(EnterCoro());
    }

    private void OnEnable()
    {
        if (automaticJuiceOnEnable)
            StartCoroutine(EnterCoro());
    }

    private void OnDisable()
    {
        StopCoroutine(EnterCoro());
        StopCoroutine(ExitCoro());
    }

    public void Enter()
    {
        StartCoroutine(EnterCoro());
    }

    IEnumerator EnterCoro()
    {
        if (deactivateAfterExit)
            gameObject.SetActive(true);

        yield return new WaitForSeconds(enterDelay);
        float n = 0;
        float startTime = Time.time;
        do
        {
            n = (Time.time - startTime) / duration;
            if (juiceScale)
                transform.localScale = Vector3.LerpUnclamped(startScale, endScale, lerpCurve.Evaluate(n));
            if (juicePosition)
                transform.localPosition = Vector3.LerpUnclamped(startPos, EndPos, lerpCurve.Evaluate(n));

            yield return null;
        }
        while (n < 1);

        if (idleAfterEntrance)
            if (im != null)
                im.Idle();
    }

    public void Exit()
    {
        StartCoroutine(ExitCoro());
    }

    IEnumerator ExitCoro()
    {
        yield return new WaitForSeconds(exitDelay);
        float n = 0;
        float startTime = Time.time;
        do
        {
            n = (Time.time - startTime) / duration;
            if (juiceScale)
                transform.localScale = Vector3.LerpUnclamped(endScale, startScale, lerpCurve.Evaluate(n));
            if (juicePosition)
                transform.localPosition = Vector3.LerpUnclamped(EndPos, startPos, lerpCurve.Evaluate(n));

            yield return null;
        }
        while (n < 1);

        if (idleAfterEntrance)
            if (im != null)
                im.Idle();

        if (deactivateAfterExit)
            gameObject.SetActive(false);
    }
}

