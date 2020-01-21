using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleMotion : MonoBehaviour
{

    public float motionAmount = 1;
    private Vector3 startPos;
    Vector3 tempPos;

    public void Idle()
    {
        StartCoroutine(Idler());
    }

    IEnumerator Idler()
    {
        startPos = this.transform.position;
        while (true)
        {
            tempPos.y = startPos.y + Mathf.Sin(Time.time) * motionAmount;
            //tempPos.x = startPos.x + Mathf.Cos(Time.time) * motionAmount;
            transform.position = tempPos;
            yield return new WaitForEndOfFrame();
        }
    }
}
