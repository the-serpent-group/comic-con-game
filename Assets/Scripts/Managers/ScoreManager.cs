using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;
    LevelManager levelManager;

    private void Start()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    void FixedUpdate()
    {
        if (levelManager.currentPhase != LevelManager.DayPhase.Morning && levelManager.currentPhase != LevelManager.DayPhase.Night)
        {
            score += 1; //per update = per frame +1 . score += fps. (universal portability) fps annd performance plays a role in this.
            scoreText.text = score.ToString(); //updates the ui

        }
    }
}
