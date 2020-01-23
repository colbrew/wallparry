using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public Tutorial[] tutorials;

    private void Awake()
    {
        foreach (Tutorial tutorial in tutorials)
        {
            tutorial.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Level.Current.tutorialOn)
        {
            tutorials[0].gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
