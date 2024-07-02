using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CheckpointOverride : MonoBehaviour
{

    private TMP_InputField text;

    void Start()
    {
        text = GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (text.text != "")
        {
            FindObjectOfType<CheckpointManager>().respawnIndex = Int32.Parse(text.text);
        }
    }
}
