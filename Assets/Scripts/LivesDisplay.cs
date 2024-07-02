using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LivesDisplay : MonoBehaviour
{

    public int lives = 3;
    public string name;
    public TMP_Text livesText;
    public TMP_Text nameText;
    public TMP_Text label;
    public Image background;
    public GameObject deadLabel;
    [Header("Default Colors")]
    public Color defaultText;
    public Color defaultBackground;
    [Header("Dead Colors")]
    public Color deadText;
    public Color deadBackground;

    // Update is called once per frame
    void Update()
    {
        if (lives > 0)
        {
            string temp = "";
            for (int i = 0; i < lives; i++)
            {
                temp += "♥";
            }
            livesText.text = temp;

            nameText.color = defaultText;
            label.color = defaultText;
            background.color = defaultBackground;
            deadLabel.SetActive(false);
        }
        else
        {
            livesText.text = "";

            nameText.color = deadText;
            label.color = deadText;
            background.color = deadBackground;
            deadLabel.SetActive(true);
        }
        nameText.text = name;
    }
}
