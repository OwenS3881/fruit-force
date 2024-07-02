using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Chronos;
using Photon.Pun;
using Photon.Realtime;

public class LevelLoader : BaseBehaviour
{

    public Animator anim;
    public float transitionTime = 1f;
    public GameObject loadingBarCanvas;
    public Slider loadingBar;
    public TMP_Text percentText;
    public bool currentlyLoading = false;

    public void LoadSceneEffect(string name)
    {
        if (!currentlyLoading)
        {
            StartCoroutine(Load(name, false));
            currentlyLoading = true;
        }
    }

    public void LoadPhotonLevel(string name)
    {
        if (!currentlyLoading)
        {
            StartCoroutine(Load(name, true));
            currentlyLoading = true;
        }
    }

    IEnumerator Load(string name, bool isPhoton)
    {
        anim.SetTrigger("End");
        yield return time.WaitForSeconds(transitionTime);
        if (!isPhoton)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(name);

            loadingBarCanvas.SetActive(true);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                loadingBar.value = progress;
                percentText.text = (progress * 100).ToString("F0") + "%";

                yield return null;
            }
        }
        else
        {
            PhotonNetwork.LoadLevel(name);
        }
    }

}
