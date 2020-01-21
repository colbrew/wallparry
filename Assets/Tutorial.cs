using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{   
    public UIJuicer[] instructions;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("RunTutorial");
    }

    // Update is called once per frame
    IEnumerator RunTutorial()
    {
        instructions[0].Enter();
        while(!Player.Current.IsParrying || Player.Current.SuperParry)
        {
            yield return null;
        }
        instructions[0].Exit();
        instructions[1].Enter();
        yield return new WaitForSeconds(1);
        instructions[1].Exit();
        instructions[2].Enter();
        while(!Player.Current.Pulsing)
        {
            yield return null;
        }
        instructions[3].Enter();
        while(!Player.Current.SuperParry)
        {
            yield return null;
        }
        instructions[2].Exit();
        instructions[3].Exit();
        instructions[4].Enter();
        yield return new WaitForSeconds(3);
        instructions[4].Exit();

        yield return null;
    }
}
