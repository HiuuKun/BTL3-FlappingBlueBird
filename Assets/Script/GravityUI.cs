using UnityEngine;
using TMPro;

public class GravityUI : MonoBehaviour
{
    public BirdController birdController;
    public TextMeshProUGUI gravityText;

    void Update()
    {
        if (gravityText == null)
        {
            return;
        }

        if (birdController != null && birdController.isGravityMode)
        {
            float remaining = Mathf.Max(0f, birdController.GravityModeRemainingTime);
            gravityText.text = "Gravity: " + remaining.ToString("F1") + "s";
            gravityText.gameObject.SetActive(true);
        }
        else
        {
            gravityText.gameObject.SetActive(false);
        }
    }
}
