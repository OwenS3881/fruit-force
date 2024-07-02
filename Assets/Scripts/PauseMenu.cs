using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public bool gamePaused = false;
    public TMP_Text warpDisplayText;
    public CheckpointManager checkpointManager;
    public int warpIndex;

    public void Pause()
    {
      this.gameObject.SetActive(true);
      SetTime(0f);
      gamePaused = true;
    }

    public void Resume()
    {
      this.gameObject.SetActive(false);
      SetTime(1f);
      gamePaused = false;
    }

    void SetTime(float val)
    {
        Timekeeper.instance.Clock("Root").localTimeScale = val;
        Timekeeper.instance.Clock("Banana").localTimeScale = val;
        Timekeeper.instance.Clock("DeathEffects").localTimeScale = val;
        if (FindObjectOfType<SpeedrunTimer>() != null)
        {
            Timekeeper.instance.Clock("Speedrun").localTimeScale = val;
        }
    }

    public void ClickSound()
    {
        AudioManager.instance.PlaySoundOneShot("Click");
    }

    private void OnEnable()
    {
        warpIndex = checkpointManager.respawnIndex;
    }

    private void FixedUpdate()
    {
        warpDisplayText.text = (warpIndex+1).ToString();
    }

    public void ChangeWarp(int val)
    {
        warpIndex += val;
        if (warpIndex > checkpointManager.highestCheckpoint)
        {
            warpIndex = 0;
        }
        else if (warpIndex < 0)
        {
            warpIndex = checkpointManager.highestCheckpoint;
        }
    }

    public void WarpToCheckpoint()
    {
        FindObjectOfType<SpeedrunTimer>().speedrunActive = false;
        checkpointManager.ClearActivate();
        checkpointManager.respawnIndex = warpIndex;
        checkpointManager.UpdateFromData();
        FindObjectOfType<RespawnManager>().respawnType = "Checkpoint";
        FindObjectOfType<DataManager>().SaveData();
        FindObjectOfType<LevelLoader>().LoadSceneEffect("MainLevel");
    }

}
