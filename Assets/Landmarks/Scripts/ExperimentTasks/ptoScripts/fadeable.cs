using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeable : MonoBehaviour
{

    private bool fadeOut, fadeIn, destroyAfterFadeOut;

    public float fadeSpeed = 1.2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        float colorDirection;

        colorDirection = 0;
        if(fadeOut)
        {
            colorDirection = -1;
        }

        if(fadeIn)
        {
            colorDirection = 1;
        }

        Color objectColor = this.GetComponent<Renderer>().material.color;
        float newAlpha = objectColor.a + (fadeSpeed * Time.deltaTime * colorDirection);

        Color newColor = new Color(objectColor.r, objectColor.g, objectColor.b, newAlpha);
        this.GetComponent<Renderer>().material.color = newColor;
        
        if(fadeOut && newAlpha <= 0)
        {
            fadeOut = false;
            if(destroyAfterFadeOut){
                Destroy(this.transform.gameObject);
            }
        }
        
        if(fadeIn && newAlpha >= 1)
        {
            fadeIn = false;
        }
        
    }

    public void FadeOut()
    {
        fadeOut = true;
    }

    public void FadeOutThenDestroy()
    {
        fadeOut = true;
        destroyAfterFadeOut = true;
    }

    public void FadeIn()
    {
        fadeIn = true;
    }
}
