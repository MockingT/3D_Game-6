
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRecorder
{
    // show the score
    private int _score = -1;
    public Text scoreText;
    public void setActive()
    {
        scoreText.text = "Score: " + _score;
    }
    public void resetScore()
    {
        _score = -1;
    }
    public void addScore(int addscore)
    {
        _score += addscore;
        scoreText.text = "Score: " + _score;
    }
    public void setDisActive()
    {
        scoreText.text = "";
    }
}

