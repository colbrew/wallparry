using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
    public static LivesUI Current;

    public GameObject[] lifeUIObjects;
    int nextHeartToTurnOff;
    int numLives;

    private void Awake()
    {
        Current = this;
        numLives = Player.Current.numberOfLives;
        InitHearts();
    }

    public void InitHearts()
    {
        for (int i = 0; i < lifeUIObjects.Length; i++)
        {
            if (i < numLives)
                lifeUIObjects[i].SetActive(true);
            else
                lifeUIObjects[i].SetActive(false);
        }
        nextHeartToTurnOff = numLives - 1;
    }

    private void OnEnable()
    {
        Challenge.challengeFailed += TurnOffHeart;
    }

    private void OnDisable()
    {
        Challenge.challengeFailed -= TurnOffHeart;
    }

    void TurnOffHeart()
    {
        if (nextHeartToTurnOff >= 0)
        {
            lifeUIObjects[nextHeartToTurnOff].SetActive(false);
            nextHeartToTurnOff -= 1;
        }
    }
}
