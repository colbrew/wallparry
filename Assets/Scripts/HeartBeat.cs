using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeat : MonoBehaviour
{
    public float maxScale;
    public float minScale;
    public float animationTime;
    float currentTime;
    public AnimationCurve animationCurve;

    void Start()
    {
        
    }

    void Update()
    {
        if(currentTime < animationTime)
        {
            currentTime += Time.deltaTime;
            Debug.Log("we here");
            float timePercent = currentTime/animationTime;
            float curvePercent = animationCurve.Evaluate(timePercent);

            float scale = Mathf.Lerp(minScale, maxScale, curvePercent);

            transform.localScale = Vector3.one * scale;
        }

    }

    public void AnimateBeat()
    {
        currentTime = 0f;
    }

    public void SetAnimationTime(float bpm)
    {
        animationTime = (60.0f / bpm);
    }
}
