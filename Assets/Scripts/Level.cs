﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Level : MonoBehaviour
{
	public static Level Current { get; private set; }

	[System.Serializable]
	public class DifficultyLevels
	{
		public float timePassed = 0.0f;
		public int easyWeight = 100;
		public int mediumWeight = 0;
		public int hardWeight = 0;

		public float challengesPerMeasure = 1;

		public Challenge.Difficulty GetRandomDifficulty()
		{
			var r = Random.Range(0, easyWeight + mediumWeight + hardWeight);

			if (r < easyWeight)
				return Challenge.Difficulty.Easy;
			else if (r < easyWeight + mediumWeight)
				return Challenge.Difficulty.Medium;
			else
				return Challenge.Difficulty.Hard;
		}
		// TODO: add random pauses
	}
	public HeartBeat heartBeat;
	public List<DifficultyLevels> difficultyLevels;
	public List<Challenge> challenges;
	public float startupTime;
	public Transform beatDebugger;
	public float bpm = 120.0f;
    public bool tutorialOn = true;
    public GameObject gameOverText;
	public bool gameActive = true;

	private Dictionary<Challenge.Difficulty, List<Challenge>> challengesPerDifficulty = new Dictionary<Challenge.Difficulty, List<Challenge>>();

	private float startTime = -1.0f;
	int previousEightNote = 0;

	void Awake()
    {
		Current = this;
		if(heartBeat != null)
			heartBeat.SetAnimationTime(bpm);
        Player.Current.SetChargedFlashRate(bpm);
        gameOverText.SetActive(false);
		difficultyLevels.Sort((a, b) => b.timePassed.CompareTo(a.timePassed));
		foreach (var c in challenges)
		{
			if (!challengesPerDifficulty.ContainsKey(c.difficulty))
				challengesPerDifficulty.Add(c.difficulty, new List<Challenge>());

			challengesPerDifficulty[c.difficulty].Add(c);
		}

	}

    private void OnDestroy()
	{
		Current = null;
	}

	void Start()
	{
		beatDebugger.gameObject.SetActive(false);
        if (tutorialOn)
        {
			TutorialController.Instance.PlayTutorial();
        }
        else
        {
			StartGame();
        }
	}

    public void StartGame()
    {
        StartCoroutine("GameStart");
    }

    IEnumerator GameStart()
    {
		AudioManager.Instance.PlayGameMusic();
		HeartBeat.Instance.AnimateBeat();

		yield return new WaitForSeconds(startupTime);

		startTime = Time.time;
    }

	private void Update()
	{
		// Check every frame if a eight note has passed and try to spawn a challenge
		if (startTime > 0 && gameActive)
		{
			float timePassed = Time.time - startTime;
			int eightNote = Mathf.FloorToInt(timePassed / ((60.0f / bpm) / 8.0f) );
			beatDebugger.gameObject.SetActive(eightNote % 8 == 0);
			if(eightNote % 8 == 0 && heartBeat != null)
				heartBeat.AnimateBeat();
			if (eightNote != previousEightNote)
			{
				previousEightNote = eightNote;
				TrySpawnChallengePrefab();
			}
		}
	}

	DifficultyLevels GetCurrentDifficultyLevel()
	{
		float timePassed = Time.time - startTime;
		return difficultyLevels.Find((d) => d.timePassed <= timePassed);
	}

	void TrySpawnChallengePrefab()
	{
		var difficultySetup = GetCurrentDifficultyLevel();
		if (difficultySetup != null)
		{
			float chance = difficultySetup.challengesPerMeasure / 8.0f;

			if (Random.value <= chance)
			{
				var difficulty = difficultySetup.GetRandomDifficulty();

				var lst = challengesPerDifficulty[difficulty];
				GameObject.Instantiate<Challenge>(lst[Random.Range(0, lst.Count)], this.transform);
				//Debug.Log("SPAWN AT: " + (Time.time - startTime).ToString());
			}
		}
	}

    public void EndGame()
    {
        gameOverText.SetActive(true);
		gameActive = false;
		Player.Current.paused = true;
		Player.Current.transform.position = Vector3.zero;
		GameObject[] remainingChallenges = GameObject.FindGameObjectsWithTag("ChallengeWall");
		foreach (GameObject go in remainingChallenges)
			Destroy(go);
    }

    public void RestartGame()
    {
		gameOverText.SetActive(false);
		gameActive = true;
		Player.Current.RestartGame();
		LivesUI.Current.InitHearts();
		ScoreUI.Instance.InitNewGame();
    }

}
