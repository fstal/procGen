using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    GameObject[] pauseObjects;
    private MeshGenerator meshScript;
    public Slider scaleSlider, redistributionSlider, lacunaritySlider;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        HidePause();
        meshScript = GameObject.FindGameObjectWithTag("MeshGenerator").GetComponent<MeshGenerator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Pressed P");
            if(Time.timeScale == 1)
            {
                Time.timeScale = 0;
                ShowPaused();
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                HidePause();
            }
        }
    }

    public void ShowPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }

    public void HidePause()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    public void NewScale()
    {
        scaleSlider = GameObject.Find("ScaleSlider").GetComponent<Slider>();
        meshScript.scale = scaleSlider.value;
    }

    public void NewRedistribution()
    {   
        redistributionSlider = GameObject.Find("RedistributionSlider").GetComponent<Slider>();
        meshScript.redistribution = redistributionSlider.value;
    }

    //public void NewLacunarity()
    //{
    //    lacunaritySlider = GameObject.FindGameObjectWithTag("Lacunarity").GetComponent<Slider>();
    //    meshScript.lacunarity = lacunaritySlider.value;
    //}

}