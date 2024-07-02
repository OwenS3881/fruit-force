using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using DialogueEditor;
using UnityEngine.UI;

public class PineappleDialogue : MonoBehaviour
{
    public NPCConversation[] dialogues;


    // Start is called before the first frame update
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

    public void SetTime(float val)
    {
        Timekeeper.instance.Clock("Root").localTimeScale = val;
        Timekeeper.instance.Clock("Banana").localTimeScale = val;
        Timekeeper.instance.Clock("DeathEffects").localTimeScale = val;
    }

    public void ReturnToChar()
    {
        FindObjectOfType<LevelLoader>().LoadSceneEffect("CharacterSelect");
    }

    IEnumerator Dialogue()
    {
        SC(dialogues[0]);
        yield return new WaitForSeconds(0.1f);
        while (FindObjectOfType<PineapplePowerupController>().currentOres != 0)
        {
            yield return new WaitForSeconds(0.000000000001f);
        }
        EC();
        yield return new WaitForSeconds(0.5f);
        SC(dialogues[1]);
    }
}
