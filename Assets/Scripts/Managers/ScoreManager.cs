using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int score = 0; 
    public TextMeshProUGUI scoreText;

    void Update()
    {
        score += 1; //per update = per frame +1 . score += fps. (universal portability) fps annd performance plays a role in this.
        scoreText.text = "Score: " + score; //updates the ui
    }
}
