using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(Rigidbody2D))]
public class ScoreBurst : MonoBehaviour
{
    Rigidbody2D rb;
    [Range(0,1)]
    public float fadeSpeed = 1;
    Text text;
    Color startTextColor;

    private void Awake()
    {
        rb = GetComponent <Rigidbody2D> ();
        text = GetComponent<Text>();
        startTextColor = text.color;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        rb.AddForce(new Vector2(Random.Range(-2.0f,2.0f), Random.Range(3.0f, 5.0f)),ForceMode2D.Impulse);
        StartCoroutine("FadeText");
    }

    public void SetText(int points)
    {
        text.text = points.ToString();
    }

    IEnumerator FadeText()
    {
        float u = 0;
        float z = 0;
        Color endColor = startTextColor;
        endColor.a = 0;
        while(z < 1)
        {

            text.color = Color.Lerp(startTextColor, endColor, z);
            u += Time.deltaTime * fadeSpeed;
            z = u * u;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
