using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeable : MonoBehaviour
{

    private bool fadeOut, fadeIn, destroyAfterFadeOut, disableAfterFadeOut;
    private Material mat, afterTransitionMaterial;
    public float fadeSpeed = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // DJH turning this into an if statement, since we don't need to run this if not changing alpha/color
        if (fadeIn || fadeOut)
        {
            mat = this.GetComponent<Renderer>().material;
            float colorDirection;

            colorDirection = 0;
            if (fadeOut)
            {
                colorDirection = -1;
            }

            if (fadeIn)
            {
                colorDirection = 1;
            }

            Color objectColor = mat.color;
            float newAlpha = objectColor.a + (fadeSpeed * Time.deltaTime * colorDirection);

            //if (fadeIn && notIncreasedBrightnessYet)
            //{
            //    newAlpha = 0.0f + (fadeSpeed * Time.deltaTime * colorDirection);
            //    notIncreasedBrightnessYet = false;
            //}

            Debug.Log("GAMEOBJECT");
            Debug.Log(this.gameObject.name);
            Debug.Log("COLORDIRECTION");
            Debug.Log(colorDirection);
            Debug.Log("NEWALPHA");
            Debug.Log(newAlpha);

            Color newColor = new Color(objectColor.r, objectColor.g, objectColor.b, newAlpha);
            mat.color = newColor;

            if (fadeOut && newAlpha <= 0)
            {
                Debug.Log("WE ARE FADING OUT HERE!!!");
                Debug.Log(this.transform.gameObject.name);
                fadeOut = false;
                if (destroyAfterFadeOut)
                {
                    Destroy(this.transform.gameObject);
                }
                if (afterTransitionMaterial)
                {
                    this.transform.gameObject.GetComponent<Renderer>().material = afterTransitionMaterial;
                    afterTransitionMaterial = null;
                }
                if (disableAfterFadeOut)
                {
                    this.transform.gameObject.SetActive(false);
                    disableAfterFadeOut = false;
                }
            }

            if (fadeIn && newAlpha >= 1)
            {
                fadeIn = false;
                // DJH edits
                //notIncreasedBrightnessYet = true;
                // DJH: IDK why but this fixes bug of door/boundaries fading after fadeIn
                fadeOut = false;
            }
        }
    }

    public void FadeOutThenDisable()
    {
        fadeOut = true;
        disableAfterFadeOut = true;
    }

    /*
        Overload that allows to specify two materials; one for the object to have during the transition,
        and to swap back to afterwards.
    */
    public void FadeOutThenDisable(Material duringTransition, Material afterTransition)
    {
        this.transform.gameObject.GetComponent<Renderer>().material = duringTransition;
        fadeOut = true;
        disableAfterFadeOut = true;
        afterTransitionMaterial = afterTransition;
    }

    public void FadeOutThenDestroy()
    {
        fadeOut = true;
        destroyAfterFadeOut = true;
    }

    public void EnableThenFadeIn()
    {
        // DJH add code here to start the thing at a relatively low alpha/brightness
        mat = this.GetComponent<Renderer>().material;
        Color objectColor = mat.color;
        float newAlpha = objectColor.a * 0.01f;
        mat.color = new Color(objectColor.r, objectColor.g, objectColor.b, newAlpha);

        this.transform.gameObject.SetActive(true);
        Debug.Log("WE ARE SETTING THE FOLLOWING GAME OBJECT TO BE ACTIVE");
        Debug.Log(this.transform.gameObject.name);
        Debug.Log("IS THE THING SET TO ACTIVE FOLLOWING THIS CHANGE???");
        Debug.Log(this.transform.gameObject.activeSelf);
        fadeIn = true;
    }

}
