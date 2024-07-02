using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class DataManager : MonoBehaviour
{

    public GameObject banana;
    public GameObject melon;
    public GameObject cherry;
    public GameObject pineapple;
    public GameObject dragonfruit;
    public GameObject coconut;
    public string loader;
    private CharacterSelectManager charMan;

    public void SaveData()
    {
      FindObjectOfType<DataCollector>().FindData();
      SaveSystem.SaveData(FindObjectOfType<DataCollector>());
    }

    void OnEnable()
    {
        LoadRespawnData();
        if (loader == "Level")
        {
            LoadLevelData();
        }
        else if (loader == "Character")
        {
            LoadSelectData();
        }
        else if (loader == "Try")
        {
            LoadTryData();
        }
    }

    public void CharSelect()
    {
      FindObjectOfType<DataManager>().SaveData();
      FindObjectOfType<LevelLoader>().LoadSceneEffect("CharacterSelect");
    }

    public void LoadLevelData()
    {
      GameData gameData = SaveSystem.LoadData();
      banana.SetActive(gameData.playerType == "Banana");
      melon.SetActive(gameData.playerType == "Melon");
      cherry.SetActive(gameData.playerType == "Cherry");
      pineapple.SetActive(gameData.playerType == "Pineapple");
      dragonfruit.SetActive(gameData.playerType == "Dragonfruit");
        coconut.SetActive(gameData.playerType == "Coconut");
      FindObjectOfType<CheckpointManager>().respawnIndex = gameData.activatedIndex;
        FindObjectOfType<CheckpointManager>().highestCheckpoint = gameData.highestCheckpoint;
        FindObjectOfType<CheckpointManager>().UpdateFromData();
        //FindObjectOfType<SpeedrunTimer>().rawSpeedrunTime = gameData.rawSpeedrunTime;
        //FindObjectOfType<SpeedrunTimer>().speedrunActive = gameData.speedrunActive;
      if (gameData.respawnType == "Checkpoint")
      {
        banana.transform.position = FindObjectOfType<CheckpointManager>().Checkpoints[FindObjectOfType<CheckpointManager>().respawnIndex].GetComponent<Checkpoint>().respawnPoint.transform.position;
        melon.transform.position = FindObjectOfType<CheckpointManager>().Checkpoints[FindObjectOfType<CheckpointManager>().respawnIndex].GetComponent<Checkpoint>().respawnPoint.transform.position;
        cherry.transform.position = FindObjectOfType<CheckpointManager>().Checkpoints[FindObjectOfType<CheckpointManager>().respawnIndex].GetComponent<Checkpoint>().respawnPoint.transform.position;
        pineapple.transform.position = FindObjectOfType<CheckpointManager>().Checkpoints[FindObjectOfType<CheckpointManager>().respawnIndex].GetComponent<Checkpoint>().respawnPoint.transform.position;
        dragonfruit.transform.position = FindObjectOfType<CheckpointManager>().Checkpoints[FindObjectOfType<CheckpointManager>().respawnIndex].GetComponent<Checkpoint>().respawnPoint.transform.position;
            coconut.transform.position = FindObjectOfType<CheckpointManager>().Checkpoints[FindObjectOfType<CheckpointManager>().respawnIndex].GetComponent<Checkpoint>().respawnPoint.transform.position;
        }
      else if (gameData.respawnType == "Safe")
      {
        banana.transform.position = new Vector3 (gameData.lastSafePos[0], gameData.lastSafePos[1], gameData.lastSafePos[2]);
        melon.transform.position = new Vector3 (gameData.lastSafePos[0], gameData.lastSafePos[1], gameData.lastSafePos[2]);
        cherry.transform.position = new Vector3 (gameData.lastSafePos[0], gameData.lastSafePos[1], gameData.lastSafePos[2]);
        pineapple.transform.position = new Vector3(gameData.lastSafePos[0], gameData.lastSafePos[1], gameData.lastSafePos[2]);
        dragonfruit.transform.position = new Vector3(gameData.lastSafePos[0], gameData.lastSafePos[1], gameData.lastSafePos[2]);
            coconut.transform.position = new Vector3(gameData.lastSafePos[0], gameData.lastSafePos[1], gameData.lastSafePos[2]);
            FindObjectOfType<BlasterPoint>().lastSafePos = new Vector3(gameData.lastSafePos[0], gameData.lastSafePos[1], gameData.lastSafePos[2]);
      }

        //Ore stuff
        Ore[] ores = Resources.FindObjectsOfTypeAll(typeof(Ore)) as Ore[];
        if (gameData.unCollectedOreIDs[0] == "RESET")
        {
            for (int i = 0; i < ores.Length; i++)
            {
                ores[i].collected = false;
            }
            return;
        }
        for (int i = 0; i < ores.Length; i++)
        {
            ores[i].collected = !gameData.unCollectedOreIDs.Contains(ores[i].myGuid);
        }
    }

    public void LoadTryData()
    {
        if (banana != null)
        {
            banana.SetActive(SceneManager.GetActiveScene().name == "BananaTest");
            banana.transform.position = FindObjectOfType<Checkpoint>().respawnPoint.transform.position;
        }

        if (melon != null)
        { 
            melon.SetActive(SceneManager.GetActiveScene().name == "MelonTest");
            melon.transform.position = FindObjectOfType<Checkpoint>().respawnPoint.transform.position;
        }

        if (cherry != null)
        {
            cherry.SetActive(SceneManager.GetActiveScene().name == "CherryTest");
            cherry.transform.position = FindObjectOfType<Checkpoint>().respawnPoint.transform.position;
        }

        if (pineapple != null)
        {
            pineapple.SetActive(SceneManager.GetActiveScene().name == "PineappleTest");
            pineapple.transform.position = FindObjectOfType<Checkpoint>().respawnPoint.transform.position;
        }

        if (dragonfruit != null)
        {
            dragonfruit.SetActive(SceneManager.GetActiveScene().name == "DragonfruitTest");
            dragonfruit.transform.position = FindObjectOfType<Checkpoint>().respawnPoint.transform.position;
        }

        if (coconut != null)
        {
            coconut.SetActive(SceneManager.GetActiveScene().name == "CoconutTest");
            coconut.transform.position = FindObjectOfType<Checkpoint>().respawnPoint.transform.position;
        }
    }

    public void LoadSelectData()
    {
      GameData gameData = SaveSystem.LoadData();
        if (gameData != null)
        {
            Debug.Log("Data successfully loaded");
            if (FindObjectOfType<CheckpointManager>() != null)
            {
                FindObjectOfType<CheckpointManager>().respawnIndex = gameData.activatedIndex;
                FindObjectOfType<CheckpointManager>().highestCheckpoint = gameData.highestCheckpoint;
            }
            if (FindObjectOfType<CharacterSelectManager>() != null)
            {
                charMan = FindObjectOfType<CharacterSelectManager>();
                charMan.selected = gameData.playerType;
                charMan.selectedIndex = charMan.options.IndexOf(charMan.selected);
                charMan.currentScreenIndex = charMan.selectedIndex;
                charMan.toggle.isOn = true;
                charMan.mainCameraPositioner.transform.position = charMan.Screens[charMan.currentScreenIndex].transform.position;
                charMan.currentCheckpoint = gameData.highestCheckpoint;
                charMan.unlocked = gameData.unlocked.ToList();
                
            }
        }
        else
        {
            Debug.LogWarning("Data was null, attempting save and load");
            FindObjectOfType<DataManager>().SaveData();
            LoadSelectData();
        }
    }

    public void LoadRespawnData()
    {
        GameData gameData = SaveSystem.LoadData();
        if (FindObjectOfType<RespawnManager>() != null && gameData != null)
        {
            FindObjectOfType<RespawnManager>().adCredits = gameData.adCredits;
            FindObjectOfType<RespawnManager>().oreCount = gameData.oreCount;
        }
    }

}
