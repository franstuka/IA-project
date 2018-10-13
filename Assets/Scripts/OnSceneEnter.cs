using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class OnSceneEnter : MonoBehaviour {

    public Image BlackImage;
    public float FadeTime;
    private bool isFading = true;
    private float alpha = 1;
    private float alphaSpeed;

    // Update is called once per frame
    void Start ()
    {
        alphaSpeed = 1 / FadeTime;
        StartFade();

    }


    public void StartFade()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        //yield return new WaitForSeconds(0.40f);
        while (isFading)
        {
            alpha -= alphaSpeed * Time.deltaTime;
            if (alpha <= 0)
            {
                alpha = 0;
                isFading = false;
            }
            Color myAlphaColor = BlackImage.color;
            myAlphaColor.a = alpha;

            BlackImage.color = myAlphaColor;
            yield return new WaitForFixedUpdate();
        } 
        alpha = 1;
        isFading = true;
    }

}
