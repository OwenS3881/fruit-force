using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using DialogueEditor;
using UnityEngine.UI;
using Cinemachine;

public class CoconutDialogue : MonoBehaviour
{

    public NPCConversation[] dialogues;

    void Start()
    {
        StartCoroutine(Dialogue());
    }

    public void SC(NPCConversation c)
    {
        ConversationManager.Instance.StartConversation(c);
    }

    public void EC()
    {
        ConversationManager.Instance.EndConversation();
    }

    public void ReturnToChar()
    {
        FindObjectOfType<LevelLoader>().LoadSceneEffect("CharacterSelect");
    }

    IEnumerator Dialogue()
    {
        SC(dialogues[0]);
        while (FindObjectOfType<CoconutPowerupController>().currentOres != 0)
        {
            yield return new WaitForSeconds(0.000000000001f);
        }
        yield return new WaitForSeconds(5f);
        SC(dialogues[1]);
    }

}
