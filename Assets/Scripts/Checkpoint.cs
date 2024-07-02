using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{

    public Collider2D trigger;
    public GameObject on;
    public GameObject off;
    public GameObject respawnPoint;
    public bool activated = false;
    public int checkpointNum;
    [Header("Values Needed only for checkpoints that unlock characters")]
    public string character;
    public string sceneName;
    public CheckpointTryNow checkpointTryNow;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainLevel")
        {
            StartCoroutine(Data());
        }
    }

    IEnumerator Data()
    {
        while (!activated)
        {
            yield return null;
        }
        //Activated
        if (FindObjectOfType<DataManager>() != null)
        {
            FindObjectOfType<DataManager>().SaveData();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      if (other.tag == "Player")
      {
            if (SceneManager.GetActiveScene().name == "MainLevel" && !activated)
            {
                Debug.Log("Loggly: " + "Reached checkpoint: " + checkpointNum.ToString() + "! " + "Id: " + SystemInfo.deviceUniqueIdentifier);
                if (character != "" && sceneName != "" && checkpointTryNow != null)
                {
                    checkpointTryNow.character = character;
                    checkpointTryNow.sceneName = sceneName;
                    checkpointTryNow.Activate();
                }
            }
            if (!activated)
            {
                AudioManager.instance.PlaySound("Checkpoint");
            }

            activated = true;
       }
    }

    void Update()
    {
      on.SetActive(activated);
      off.SetActive(!activated);
    }
}
