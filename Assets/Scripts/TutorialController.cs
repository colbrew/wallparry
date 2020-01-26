using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public static TutorialController Instance;

    public Tutorial[] tutorials;

    private void Awake()
    {
        Instance = this;

        foreach (Tutorial tutorial in tutorials)
        {
            tutorial.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    public void PlayTutorial()
    {
            tutorials[0].gameObject.SetActive(true);
            AudioManager.Instance.PlayTutorialMusic();
    }
}
