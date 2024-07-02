using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataCollector : MonoBehaviour
{

    public string playerType;
    public int activatedIndex;
    public int highestCheckpoint;
    public string respawnType;
    public float[] lastSafePos;
    public bool[] unlocked;
    public int adCredits;
    public int oreCount;
    public string[] unCollectedOreIDs;
    private List<string> tempUnCollectedOreIDs = new List<string>();
    public float rawSpeedrunTime;
    public bool speedrunActive;

    public void FindData()
    {
      GameData gameData = SaveSystem.LoadData();
      if (FindObjectOfType<CharacterSelectManager>() != null)
      {
        playerType = FindObjectOfType<CharacterSelectManager>().selected;
      }
      else if (gameData.playerType != null)
      {
        playerType = gameData.playerType;
      }
      else
      {
            playerType = "";

      }

      if (FindObjectOfType<CheckpointManager>() != null)
      {
        activatedIndex = FindObjectOfType<CheckpointManager>().respawnIndex;
      }
      else if (gameData.activatedIndex != null)
      {
        activatedIndex = gameData.activatedIndex;
      }
      else
      {
            activatedIndex = 0;
      }

        if (FindObjectOfType<CheckpointManager>() != null)
        {
            highestCheckpoint = FindObjectOfType<CheckpointManager>().highestCheckpoint;
        }
        else if (gameData.highestCheckpoint != null)
        {
            highestCheckpoint = gameData.highestCheckpoint;
        }
        else
        {
            highestCheckpoint = 0;
        }

        if (FindObjectOfType<RespawnManager>() != null)
      {
        respawnType = FindObjectOfType<RespawnManager>().respawnType;
      }
      else
      {
        respawnType = "Checkpoint";
      }

      if (FindObjectOfType<BlasterPoint>() != null)
      {
        lastSafePos = new float[3];
        lastSafePos[0] = FindObjectOfType<BlasterPoint>().lastSafePos.x;
        lastSafePos[1] = FindObjectOfType<BlasterPoint>().lastSafePos.y;
        lastSafePos[2] = FindObjectOfType<BlasterPoint>().lastSafePos.z;
      }
      else if (gameData.lastSafePos != null)
      {
        lastSafePos = gameData.lastSafePos;
      }
      else
      {
            lastSafePos = new float[3];
            lastSafePos[0] = 0f;
            lastSafePos[1] = 0f;
            lastSafePos[2] = 0f;
      }

        if (FindObjectOfType<CharacterSelectManager>() != null)
        {
            CharacterSelectManager charMan = FindObjectOfType<CharacterSelectManager>();
            unlocked = new bool[charMan.unlocked.Count];
            for (int i = 0; i < unlocked.Length; i++)
            {
                unlocked[i] = charMan.unlocked[i];
            }
        }
        else if (gameData.unlocked != null)
        {
            unlocked = gameData.unlocked;
        }
        else
        {
            unlocked = new bool[10];

        }

        if (FindObjectOfType<RespawnManager>() != null)
        {
            adCredits = FindObjectOfType<RespawnManager>().adCredits;
        }
        else if (gameData.adCredits != null)
        {
            adCredits = gameData.adCredits;
        }
        else
        {
            adCredits = 0;
        }

        if (FindObjectOfType<RespawnManager>() != null)
        {
            oreCount = FindObjectOfType<RespawnManager>().oreCount;
        }
        else if (gameData.oreCount != null)
        {
            oreCount = gameData.oreCount;
        }
        else
        {
            oreCount = 0;
        }


        Ore[] ores = Resources.FindObjectsOfTypeAll(typeof(Ore)) as Ore[];
        tempUnCollectedOreIDs = new List<string>();
        if (SceneManager.GetActiveScene().name == "MainLevel")
        {
            for (int i = 0; i < ores.Length; i++)
            {
                if (ores[i].collected || ores[i].myGuid == "")
                {
                    continue;
                }
                tempUnCollectedOreIDs.Add(ores[i].myGuid);
            }
            unCollectedOreIDs = new string[tempUnCollectedOreIDs.Count];
            for (int i = 0; i < unCollectedOreIDs.Length; i++)
            {
                unCollectedOreIDs[i] = tempUnCollectedOreIDs[i];
            }
        }
        else if (gameData.unCollectedOreIDs != null)
        {
            unCollectedOreIDs = gameData.unCollectedOreIDs;
        }
        else
        {
            unCollectedOreIDs = new string[ores.Length];

        }

        if (FindObjectOfType<SpeedrunTimer>() != null)
        {
            rawSpeedrunTime = FindObjectOfType<SpeedrunTimer>().rawSpeedrunTime;
        }
        else if (gameData.rawSpeedrunTime != null)
        {
            rawSpeedrunTime = gameData.rawSpeedrunTime;
        }
        else
        {
            rawSpeedrunTime = 0f;
        }

        if (FindObjectOfType<SpeedrunTimer>() != null)
        {
            speedrunActive = FindObjectOfType<SpeedrunTimer>().speedrunActive;
        }
        else if (gameData.speedrunActive != null)
        {
            speedrunActive = gameData.speedrunActive;
        }
        else
        {
            speedrunActive = false;
        }

    }
}
