using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;

    public GameObject[] scorebursts;
    public GameObject scoreBurst;
    int score;
    Text scoreText;
    int nextScoreburst = 0;

    private void Awake()
    {
        Instance = this;

        scoreText = GetComponent<Text>();
        
        scorebursts = new GameObject[10];

        InitNewGame();

        // setup object pool of scorebursts
        for(int i=0; i < 10; i++)
        {
            GameObject tempBurst = Instantiate(scoreBurst,this.transform);
            tempBurst.SetActive(false);
            scorebursts[i] = tempBurst;
        }
    }

    public void InitNewGame()
    {
        scoreText.text = "0";
    }

    public void AddPoints(int points, bool scoreburst = true)
    {
        score += points;
        scoreText.text = score.ToString();
        if (scoreburst && points > 0)
        {
            ScoreBurst(points);
        }
          
    }

    void ScoreBurst(int points)
    {
        scorebursts[nextScoreburst].SetActive(true);
        scorebursts[nextScoreburst].transform.position = Player.Current.transform.position;
        scorebursts[nextScoreburst].GetComponent<ScoreBurst>().SetText(points);
        nextScoreburst++;
        if (nextScoreburst == scorebursts.Length)
        {
            nextScoreburst = 0;
        }
    }
}
