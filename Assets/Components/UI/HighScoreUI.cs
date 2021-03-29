using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreUI : Singleton<HighScoreUI>
{
    public int highScore;

    public Text highScoreCounter;


    // Start is called before the first frame update
    void Start()
    {
        highScore = 0;
        highScoreCounter = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        highScoreCounter.text = "High Score: " + highScore.ToString();
    }

    public void adjustHighScore(int newScore)
    {
        if (newScore > highScore)
            highScore = newScore;
    }
}

