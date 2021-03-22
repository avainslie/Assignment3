using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NestUI : Singleton<NestUI>
{

    public Text nestBlockCounter;

    private int nestBlockCount;

    
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
}
