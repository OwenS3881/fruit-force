using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using TMPro;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RespawnManager : MonoBehaviour, IUnityAdsListener
{
#if UNITY_IOS
    string gameId = "4221048";
    string rewardedAdName = "SafeRespawnIOS";
#else
    string gameId = "4221049";
    string rewardedAdName = "SafeRespawnAndroid";
#endif

    public string respawnType;
    public bool testMode = true;
    public int adCredits;
    public GameObject adCanvas;
    public TMP_Text adCreditText;
    public string adCreditID;
    public int oreCount;
    [SerializeField] int oreConversionRate = 50;
    public GameObject respawnCreditButton;
    [Header("Only in Main Level")]
    public TMP_Text oreCountText;
    public Transform respawnCreditPlusPos;
    public GameObject respawnCreditPlusEffect;
    
    private void Start()
    {
        Advertisement.Initialize(gameId, testMode);
        Advertisement.AddListener(this);
    }

    public void RespawnCheckpoint()
    {
      respawnType = "Checkpoint";
      FindObjectOfType<DataManager>().SaveData();
      FindObjectOfType<LevelLoader>().LoadSceneEffect("MainLevel");
    }

    public void ShowSafeRespawnMenu()
    {
        adCanvas.SetActive(true);
    }

    public void HideSafeRespawnMenu()
    {
        adCanvas.SetActive(false);
    }

    public void AdRespawn()
    {
        if (Advertisement.IsReady(rewardedAdName))
        {
            Advertisement.Show(rewardedAdName);
        }
        else
        {
            Debug.Log("Ad not ready");
            return;
        }
    }

    public void CreditRespawn()
    {
        if (adCredits > 0)
        {
            adCredits--;
            RewardAddFinish();
        }
    }

    public void ClickSound()
    {
        AudioManager.instance.PlaySoundOneShot("Click");
    }

    public void RewardAddFinish()
    {
        respawnType = "Safe";
        FindObjectOfType<DataManager>().SaveData();
        FindObjectOfType<LevelLoader>().LoadSceneEffect("MainLevel");
    }

    public void CharacterSelect()
    {
      respawnType = "Checkpoint";
      FindObjectOfType<DataManager>().SaveData();
      FindObjectOfType<LevelLoader>().LoadSceneEffect("CharacterSelect");
    }

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == adCreditID)
        {
            adCredits += 20;
            FindObjectOfType<DataManager>().SaveData();
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError("Purchase of " + product.definition.id + " failed due to " + reason);
    }

    void FixedUpdate()
    {
        if (adCreditText != null)
        {
            adCreditText.text = "You have: " + adCredits.ToString() + " Respawn Credits";
        }
        if (oreCountText != null && SceneManager.GetActiveScene().name == "MainLevel")
        {
            oreCountText.text = oreCount.ToString();
        }
        if (oreCount >= oreConversionRate && respawnCreditPlusPos != null && respawnCreditPlusEffect != null)
        {
            adCredits++;
            oreCount -= oreConversionRate;
            Instantiate(respawnCreditPlusEffect, respawnCreditPlusPos.position, Quaternion.identity, respawnCreditPlusPos);
            AudioManager.instance.PlaySound("RespawnCreditPlus");
            FindObjectOfType<DataManager>().SaveData();
        }
        if (respawnCreditButton != null)
        {
            respawnCreditButton.GetComponent<Button>().interactable = adCredits != 0;
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("Ads ready :)");
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.LogError("Ads Error: " + message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Ads started :D");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == rewardedAdName && showResult == ShowResult.Finished)
        {
            RewardAddFinish();
        }
    }
}
