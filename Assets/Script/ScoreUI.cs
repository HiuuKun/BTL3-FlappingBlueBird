using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public BirdController birdController;
    public TMPro.TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (birdController != null && scoreText != null)
        {
            scoreText.text = "Score: " + birdController.experience.ToString();
        }
    }
}
