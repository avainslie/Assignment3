using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Keeps track of the current generations nest block count
/// </summary>
public class NestUI : Singleton<NestUI>
{

    public Text nestBlockCounter;

    public int nestBlockCount;

    
    // Start is called before the first frame update
    void Start()
    {
        nestBlockCount = 0;
        nestBlockCounter = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        nestBlockCounter.text = "Nest block count: " + nestBlockCount.ToString();
    }


    public void addNestBlockToCount()
    {
        nestBlockCount++;
    }

    public void resetNestUI()
    {
        nestBlockCount = 0;
    }
}
