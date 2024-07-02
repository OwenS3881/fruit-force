using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempSwapper : MonoBehaviour
{
    public Toggle toggle;
    public GameObject banana;
    public GameObject melon;

    // Update is called once per frame
    void Update()
    {
        banana.SetActive(toggle.isOn);
        melon.SetActive(!toggle.isOn);
    }
}
