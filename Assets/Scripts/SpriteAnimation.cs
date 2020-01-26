using UnityEngine;
using System;
using System.Collections;

public class SpriteAnimation : MonoBehaviour
{
    public GameObject AnimatedGameObject;
    public AnimSpriteSet[] AnimationSets;
    private int Cur_SpriteID;
    private float SecsPerFrame = 0.25f;

    void Awake()
    {
        //      Cur_SpriteID = 0;
        //  if(!AnimatedGameObject){
        //      AnimatedGameObject = this.gameObject;
        //  }
        //      PlayAnimation (0, 0.25f);
    }

    public void PlayAnimation(int ID, float secPerFrame)
    {
        SecsPerFrame = secPerFrame;
        StartCoroutine(AnimateSprite(ID, secPerFrame));

    }

    IEnumerator AnimateSprite(int ID, float secPerFrame)
    {
        for(int i = 0; i < AnimationSets[ID].Anim_Sprites.Length; i++)
        {
            AnimatedGameObject.GetComponent<SpriteRenderer>().sprite = AnimationSets[ID].Anim_Sprites[Cur_SpriteID];
            Cur_SpriteID++;
            yield return new WaitForSeconds(SecsPerFrame);
        }
    }
}

[Serializable]
public class AnimSpriteSet
{
    public string AnimationName;
    public Sprite[] Anim_Sprites;
}