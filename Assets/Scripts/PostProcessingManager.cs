using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System;
using UnityEngine.SceneManagement;

public class PostProcessingManager : MonoBehaviour
{

    Vignette vignetteLayer = null;
    Bloom bloomLayer = null;
    PostProcessVolume volume;

    public Color defaultColor;
    public float defaultIntensity;
    public float defaultSmoothness;

    public Color hitColor;
    public float hitIntensity;
    public float hitSmoothness;

    public bool startHitEffect = false;

    public float defaultBloomIntensity;
    public float dieBloomIntensity;
    public bool startDieEffect = false;

    public Color slowColor;
    public float slowIntensity;
    public float slowSmoothness;

    public bool startSlowEffect = false;
    public bool stopSlowEffect = false;

    public bool startSlowHitEffect = false;

    // Start is called before the first frame update
    void Start()
    {
      volume = GetComponent<PostProcessVolume>();
      volume.profile.TryGetSettings(out vignetteLayer);
      volume.profile.TryGetSettings(out bloomLayer);
    }

    public void SetToDefault()
    {
      vignetteLayer.color.value = defaultColor;
      vignetteLayer.intensity.value = defaultIntensity;
      vignetteLayer.smoothness.value = defaultSmoothness;
    }

    public void SetToHit()
    {
      vignetteLayer.color.value = hitColor;
      vignetteLayer.intensity.value = hitIntensity;
      vignetteLayer.smoothness.value = hitSmoothness;
    }

    public IEnumerator HitEffect(float speedB, float delayB, float speedE, float delayE)
    {
      float Time = 0f;
      while (Time < 1)
      {
        vignetteLayer.color.value = Color.Lerp(defaultColor, hitColor, Time);
        vignetteLayer.intensity.value = Mathf.Lerp(defaultIntensity, hitIntensity, Time);
        vignetteLayer.smoothness.value = Mathf.Lerp(defaultSmoothness, hitSmoothness, Time);
        Time += speedB;
        yield return new WaitForSeconds(delayB);
      }
      Time = 1;
      while (Time > 0)
      {
        vignetteLayer.color.value = Color.Lerp(defaultColor, hitColor, Time);
        vignetteLayer.intensity.value = Mathf.Lerp(defaultIntensity, hitIntensity, Time);
        vignetteLayer.smoothness.value = Mathf.Lerp(defaultSmoothness, hitSmoothness, Time);
        Time -= speedE;
        yield return new WaitForSeconds(delayE);
      }
    }

    public IEnumerator SlowHitEffect(float speedB, float delayB, float speedE, float delayE)
    {
      float Time = 0f;
      while (Time < 1)
      {
        vignetteLayer.color.value = Color.Lerp(slowColor, hitColor, Time);
        vignetteLayer.intensity.value = Mathf.Lerp(slowIntensity, hitIntensity, Time);
        vignetteLayer.smoothness.value = Mathf.Lerp(slowSmoothness, hitSmoothness, Time);
        Time += speedB;
        yield return new WaitForSeconds(delayB);
      }
      Time = 1;
      while (Time > 0)
      {
        vignetteLayer.color.value = Color.Lerp(defaultColor, hitColor, Time);
        vignetteLayer.intensity.value = Mathf.Lerp(defaultIntensity, hitIntensity, Time);
        vignetteLayer.smoothness.value = Mathf.Lerp(defaultSmoothness, hitSmoothness, Time);
        Time -= speedE;
        yield return new WaitForSeconds(delayE);
      }
    }

    public IEnumerator DieEffect(float speed, float delay)
    {
      float Time = 0;
      while (Time < 1)
      {
        bloomLayer.intensity.value = Mathf.Lerp(defaultBloomIntensity, dieBloomIntensity, Time);
        Time += speed;
        yield return new WaitForSeconds(delay);
      }
      FindObjectOfType<DataManager>().SaveData();
        AudioManager.instance.StopSound("PlayerDeath");
      SceneManager.LoadScene("RespawnMenu");
    }

    public IEnumerator StartSlowEffect(float speed, float delay)
    {
      float Time = 0f;
      vignetteLayer.rounded.value = true;
      while (Time < 1)
      {
        vignetteLayer.color.value = Color.Lerp(defaultColor, slowColor, Time);
        vignetteLayer.intensity.value = Mathf.Lerp(defaultIntensity, slowIntensity, Time);
        vignetteLayer.smoothness.value = Mathf.Lerp(defaultSmoothness, slowSmoothness, Time);
        Time += speed;
        yield return new WaitForSeconds(delay);
      }
    }

    public IEnumerator StopSlowEffect(float speed, float delay)
    {
      float Time = 1f;
      while (Time > 0)
      {
        vignetteLayer.color.value = Color.Lerp(defaultColor, slowColor, Time);
        vignetteLayer.intensity.value = Mathf.Lerp(defaultIntensity, slowIntensity, Time);
        vignetteLayer.smoothness.value = Mathf.Lerp(defaultSmoothness, slowSmoothness, Time);
        Time -= speed;
        yield return new WaitForSeconds(delay);
      }
      vignetteLayer.rounded.value = false;
    }

    void Update()
    {
      if (startHitEffect)
      {
        StartCoroutine(HitEffect(0.2f, 0.05f, 0.1f, 0.05f));
        startHitEffect = false;
      }

      if (startSlowHitEffect)
      {
        StartCoroutine(SlowHitEffect(0.2f, 0.05f, 0.1f, 0.05f));
        startSlowHitEffect = false;
      }

      if (startDieEffect)
      {
        StartCoroutine(DieEffect(0.05f, 0.05f));
        startDieEffect = false;
      }

      if (startSlowEffect)
      {
        StartCoroutine(StartSlowEffect(0.2f, 0.05f));
        startSlowEffect = false;
      }

      if (stopSlowEffect)
      {
        StartCoroutine(StopSlowEffect(0.2f, 0.05f));
        stopSlowEffect = false;
      }
    }
}
