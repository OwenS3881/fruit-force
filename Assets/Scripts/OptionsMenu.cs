using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public bool gameOptioned = false;
    public BlasterPoint[] blasters;
    public Slider camZoomSlider;
    public bool isPhoton;

    public void InitializeZoom()
    {
        if (!PlayerPrefs.HasKey("CamZoom"))
        {
            PlayerPrefs.SetFloat("CamZoom", blasters[0].camRadius);
        }
        SetSliderValue(PlayerPrefs.GetFloat("CamZoom"));
        UpdateBlasters();
    }

    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey("CamZoom"))
        {
            SetSliderValue(blasters[0].camRadius);
            PlayerPrefs.SetFloat("CamZoom", GetSliderValue());
        }
        SetSliderValue(PlayerPrefs.GetFloat("CamZoom"));
        UpdateBlasters();
    }

    void UpdateBlasters()
    {
        FindBlasters();
        foreach (BlasterPoint b in blasters)
        {
            b.camRadius = PlayerPrefs.GetFloat("CamZoom");
            if (b.isActiveAndEnabled)
            {
                b.UpdateCamRadius();
            }
        }
    }

    public void Pause()
    {
        this.gameObject.SetActive(true);
        if (!isPhoton)
        {
            SetTime(0f);
        }
        gameOptioned = true;
    }

    public void Resume()
    {
        this.gameObject.SetActive(false);
        if (!isPhoton)
        {
            SetTime(1f);
        }
        gameOptioned = false;
    }

    void SetTime(float val)
    {
        Timekeeper.instance.Clock("Root").localTimeScale = val;
        Timekeeper.instance.Clock("Banana").localTimeScale = val;
        Timekeeper.instance.Clock("DeathEffects").localTimeScale = val;
    }

    void SetSliderValue(float val)
    {
        float middle = (camZoomSlider.maxValue + camZoomSlider.minValue) / 2;
        float change = middle - val;
        camZoomSlider.value = middle + change;
    }

    float GetSliderValue()
    {
        float middle = (camZoomSlider.maxValue + camZoomSlider.minValue) / 2;
        float change = middle - camZoomSlider.value;
        return (middle + change);
    }

    public void SliderChanged()
    {
        PlayerPrefs.SetFloat("CamZoom", GetSliderValue());
        UpdateBlasters();
    }

    public void ClickSound()
    {
        AudioManager.instance.PlaySoundOneShot("Click");
    }

    void FindBlasters()
    {
        blasters = FindObjectsOfType<BlasterPoint>();
    }
}
