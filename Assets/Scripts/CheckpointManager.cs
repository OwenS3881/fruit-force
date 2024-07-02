using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{

    public List<GameObject> Checkpoints = new List<GameObject>();
    public List <bool> activatedList = new List<bool>();
    public int respawnIndex;
    public int highestCheckpoint;

    public void UpdateFromData()
    {
      for (int i = 0; i < activatedList.Count; i++)
      {
        activatedList[i] = i <= respawnIndex;
        Checkpoints[i].GetComponent<Checkpoint>().activated = activatedList[i];
      }
    }

    public void ClearActivate()
    {
      for (int i = 0; i < activatedList.Count; i++)
      {
        activatedList[i] = false;
        Checkpoints[i].GetComponent<Checkpoint>().activated = activatedList[i];
      }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Checkpoints.Count; i++)
        {
          activatedList[i] = Checkpoints[i].GetComponent<Checkpoint>().activated;
        }

        for (int i = activatedList.Count-1; i > -1; i--)
        {
          if (activatedList[i] == true)
          {
            respawnIndex = i;
            break;
          }
          else if (i == 0)
          {
            respawnIndex = 0;
            break;
          }
        }

        if (respawnIndex > highestCheckpoint)
        {
            highestCheckpoint = respawnIndex;
        }
    }
}
