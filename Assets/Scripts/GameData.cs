using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
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
    public float rawSpeedrunTime;
    public bool speedrunActive;

    public GameData (DataCollector dc)
    {
      playerType = dc.playerType;
      activatedIndex = dc.activatedIndex;
        highestCheckpoint = dc.highestCheckpoint;
      respawnType = dc.respawnType;
      lastSafePos = dc.lastSafePos;
        unlocked = dc.unlocked;
        adCredits = dc.adCredits;
        oreCount = dc.oreCount;
        unCollectedOreIDs = dc.unCollectedOreIDs;
        rawSpeedrunTime = dc.rawSpeedrunTime;
        speedrunActive = dc.speedrunActive;
    }

}
