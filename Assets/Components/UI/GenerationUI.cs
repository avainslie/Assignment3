using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerationUI : Singleton<GenerationUI>
{

    public Text generationCounter;

    public int generationCount;


    void Start()
    {
        generationCount = 1;
        generationCounter = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        generationCounter.text = "Generation: " + generationCount.ToString();
    }


    public void addGenerationToCount()
    {
        generationCount++;
    }
}