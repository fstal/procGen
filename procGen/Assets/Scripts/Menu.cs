using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    GameObject[] pauseObjects;
    private MeshGenerator meshScript;
    public ErosionScript erosionScript;
    public Slider scaleSlider, redistributionSlider, lacunaritySlider, iterationSlider, dropletSlider;
    public Text iterationText, sizeText, scaleText, redisText;

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
        scaleText = GameObject.Find("Scale").GetComponent<Text>();
        scaleText.text = scaleSlider.value.ToString();
        meshScript.scale = scaleSlider.value;
        
    }

    public void NewRedistribution()
    {   
        redistributionSlider = GameObject.Find("RedistributionSlider").GetComponent<Slider>();
        redisText = GameObject.Find("Redis").GetComponent<Text>();
        redisText.text = redistributionSlider.value.ToString();
        meshScript.redistribution = redistributionSlider.value;
    }

    public void NewNumIterations()
    {   
        iterationSlider = GameObject.Find("NumIterationsSlider").GetComponent<Slider>();
        iterationText = GameObject.Find("Iterations").GetComponent<Text>();
        iterationText.text = iterationSlider.value.ToString();
        meshScript.numIterations = (int) iterationSlider.value;
    }

    public void NewDropletSize()
    {   
        dropletSlider = GameObject.Find("DropletSlider").GetComponent<Slider>();
        sizeText = GameObject.Find("Size").GetComponent<Text>();
        sizeText.text = dropletSlider.value.ToString();
        meshScript.initialWaterVolume = dropletSlider.value;
    }


}