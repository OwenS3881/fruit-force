using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using Chronos;

public class DialogueManager : BaseBehaviour
{

    public NPCConversation[] tutorials;
    public PlayerTrigger[] tutorialTriggers;
    public Joystick[] joysticks;
    public GameObject characterBlocker;
    public GameObject characterSealer;

    // Start is called before the first frame update
    void Start()
    {
        GameData gameData = SaveSystem.LoadData();
        if (!PlayerPrefs.HasKey("TutorialSection"))
        {
            PlayerPrefs.SetInt("TutorialSection", 0);
        }
        if (gameData.activatedIndex == 0 && gameData.respawnType == "Checkpoint")
        {
            PlayerPrefs.SetInt("TutorialSection", 0);
        }
        if (PlayerPrefs.GetInt("TutorialSection") >= 5)
        {
            DisableCharacterSeal();
        }
        StartCoroutine(tutorialDialogue(PlayerPrefs.GetInt("TutorialSection")));

    }

    public void SC(NPCConversation c)
    {
        ConversationManager.Instance.StartConversation(c);
    }

    public void EC()
    {
        ConversationManager.Instance.EndConversation();
    }

    private void EnableCharacterSeal()
    {
        characterBlocker.SetActive(true);
        characterSealer.SetActive(true);
    }

    public void DisableCharacterSeal()
    {
        characterBlocker.SetActive(false);
        characterSealer.SetActive(false);
    }

    IEnumerator tutorialDialogue(int section)
    {
        if (section <= 0)
        {
            PlayerPrefs.SetInt("TutorialSection", 1);
            SC(tutorials[0]);
            yield return time.WaitForSeconds(2f);
            while (joysticks[0].Direction.sqrMagnitude == 0)
            {
                yield return time.WaitForSeconds(0.00000000000001f);
            }
            EC();
        }

        if (section <= 1)
        {
            PlayerPrefs.SetInt("TutorialSection", 2);
            while (!tutorialTriggers[0].triggered)
            {
                yield return time.WaitForSeconds(0.00000000000001f);
            }
            SC(tutorials[1]);
            yield return time.WaitForSeconds(1f);
            while (joysticks[1].Direction.sqrMagnitude == 0)
            {
                yield return time.WaitForSeconds(0.00000000000001f);
            }
            while (joysticks[1].Direction.sqrMagnitude != 0)
            {
                yield return time.WaitForSeconds(0.00000000000001f);
            }
            EC();
            SC(tutorials[2]);
        }

        if (section <= 2)
        {
            PlayerPrefs.SetInt("TutorialSection", 3);
            while (!tutorialTriggers[1].triggered)
            {
                yield return time.WaitForSeconds(0.00000000000001f);
            }
            SC(tutorials[3]);
            while (!tutorialTriggers[2].triggered)
            {
                yield return time.WaitForSeconds(0.00000000000001f);
            }
            EC();
        }

        if (section <= 3)
        {
            PlayerPrefs.SetInt("TutorialSection", 4);
            while (!tutorialTriggers[3].triggered)
            {
                yield return time.WaitForSeconds(0.00000000000001f);
            }
            SC(tutorials[4]);
        }

        if (section <= 4)
        {
            PlayerPrefs.SetInt("TutorialSection", 5);
            while (!tutorialTriggers[4].triggered)
            {
                yield return time.WaitForSeconds(0.00000000000001f);
            }
            EnableCharacterSeal();
            SC(tutorials[5]);
        }

    }

}
