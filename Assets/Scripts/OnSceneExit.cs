using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class OnSceneExit : MonoBehaviour {

    public Image BlackImage;
    public float FadeTime;
    private bool isFading = true;
    private float alpha = 0;
    private float alphaSpeed;
    [SerializeField] private GameObject[] activeThis;

    // Update is called once per frame
    void Start()
    {
        alphaSpeed = 1 / FadeTime;
       
    }

    public void StartFade()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        while(isFading)
        {
            alpha += alphaSpeed * Time.deltaTime;
            if (alpha >= 1)
            {
                alpha = 1;
                isFading = false;
            }
            Color myAlphaColor = BlackImage.color;
            myAlphaColor.a = alpha;

            BlackImage.color = myAlphaColor;
            yield return new WaitForFixedUpdate();
        }
        for (int i = 0; i < activeThis.Length; i++)
        {
            activeThis[i].SetActive(true);
        }
        alpha = 0;
        isFading = true;
    }
}
