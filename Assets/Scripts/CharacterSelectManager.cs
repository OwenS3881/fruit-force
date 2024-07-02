using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Purchasing;
using System.Linq;
using System;

public class CharacterSelectManager : MonoBehaviour
{

    public List<GameObject> Screens = new List<GameObject>();
    //public List<GameObject> Characters = new List<GameObject>();
    public List<string> options = new List<string>();
    public List<string> characterNames = new List<string>();
    [TextArea]public List<string> abilityDescriptions = new List<string>();
    public List<int> checkpointRequirement = new List<int>();
    public List<string> tutorialSceneNames = new List<string>();
    public List<bool> unlocked = new List<bool>();
    public int currentCheckpoint;
    public TMP_Text nameText;
    public TMP_Text abilityText;
    public int selectedIndex = -1;
    public Toggle toggle;
    public TMP_Text selectLabel;
    public GameObject mainCameraPositioner;
    public int currentScreenIndex = 0;
    public float camSpeed = 10f;
    [SerializeField] private bool atSpot = false;
    public string selected = "";
    public GameObject lockedPanel;
    public string[] earlyFruitIDs;
    public GameObject restorePurchaseButton;
    public Button getNowButton;
    public Button[] navButtons;
    public GameObject playMenu;

    private void Start()
    {
        restorePurchaseButton.SetActive(Application.platform == RuntimePlatform.IPhonePlayer);
    }

    public void Move(int direction)
    {
      int checkIndex = currentScreenIndex + direction;
      if (checkIndex >= 0 && checkIndex <= Screens.Count-1)
      {
        currentScreenIndex = checkIndex;
        atSpot = false;
        toggle.isOn = currentScreenIndex == selectedIndex;
      }
    }

    public void ClickSound()
    {
        AudioManager.instance.PlaySoundOneShot("Click");
    }

    public void SetNavButtons(bool b)
    {
        for (int i = 0; i < navButtons.Length; i++)
        {
            navButtons[i].interactable = b;
        }
    }

    public void ChangeToggle()
    {
      if (atSpot)
      {
        if (toggle.isOn)
        {
          selectedIndex = currentScreenIndex;
        }
        else
        {
          selectedIndex = -1;
        }
      }
        FindObjectOfType<DataManager>().SaveData();
    }

    public void Exit(string sceneName)
    {
      if (selected != "")
      {
        FindObjectOfType<DataManager>().SaveData();
        FindObjectOfType<LevelLoader>().LoadSceneEffect(sceneName);
      }
    }

    public void PlayButton()
    {
        GameData gameData = SaveSystem.LoadData();
        Vector3 lsp;
        lsp.x = gameData.lastSafePos[0];
        lsp.y = gameData.lastSafePos[1];
        lsp.z = gameData.lastSafePos[2];
        if (lsp == Vector3.zero)
        {
            Exit("MainLevel");
        }
        else
        {
            playMenu.SetActive(true);
        }
    }

    public void TryLoad()
    {
        FindObjectOfType<DataManager>().SaveData();
        FindObjectOfType<LevelLoader>().LoadSceneEffect(tutorialSceneNames[currentScreenIndex]); 
    }

    public void ResetUnlocked()
    {
        for (int i = 0; i < unlocked.Count; i++)
        {
            unlocked[i] = false;
        }
        selectedIndex = -1;
        selected = "Banana";
        toggle.isOn = false;
        FindObjectOfType<DataManager>().SaveData();
    }

    public void OnPurchaseComplete(Product product)
    {
        unlocked[Array.IndexOf(earlyFruitIDs, product.definition.id)] = true;
        FindObjectOfType<DataManager>().SaveData();
        SetNavButtons(true);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError("Purchase of " + product.definition.id + " failed due to " + reason);
        SetNavButtons(true);
    }

    // Update is called once per frame
    void Update()
    {

        getNowButton.gameObject.GetComponent<IAPButton>().productId = earlyFruitIDs[currentScreenIndex];

      if (mainCameraPositioner.transform.position != Screens[currentScreenIndex].transform.position)
      {
        mainCameraPositioner.transform.position = Vector3.MoveTowards(mainCameraPositioner.transform.position, Screens[currentScreenIndex].transform.position, camSpeed*Time.deltaTime);
      }
      else
      {
        if (!atSpot)
        {
          atSpot = true;
        }
      }

      if (atSpot)
      {
            toggle.interactable = checkpointRequirement[currentScreenIndex] <= currentCheckpoint;
      }
      else
      {
            toggle.interactable = false;
      }
        getNowButton.interactable = atSpot;
      lockedPanel.GetComponent<LockedPanel>().locked = !(checkpointRequirement[currentScreenIndex] <= currentCheckpoint);
        if (unlocked[currentScreenIndex] == true)
        {
            lockedPanel.GetComponent<LockedPanel>().locked = false;
            toggle.interactable = atSpot;
        }
        if (lockedPanel.GetComponent<LockedPanel>().locked == false)
        {
            unlocked[currentScreenIndex] = true;
        }

      if (toggle.isOn == true)
      {
        selectLabel.text = "Selected";
        toggle.interactable = false;
      }
      else
      {
        selectLabel.text = "Select?";
      }
      if (selectedIndex != -1)
      {
        selected = options[selectedIndex];
      }
      nameText.text = characterNames[currentScreenIndex];
      abilityText.text = abilityDescriptions[currentScreenIndex];
    }
}
