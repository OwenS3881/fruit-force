using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using DialogueEditor;
using UnityEngine.UI;
using Cinemachine;

public class MelonDialogue : MonoBehaviour
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
        while (!FindObjectOfType<PlayerTrigger>().triggered)
        {
            yield return new WaitForSeconds(0.000000000001f);
        }
        EC();
        SC(dialogues[1]);
        FindObjectOfType<PostProcessingManager>().startSlowEffect = true;
        FindObjectOfType<CinemachineConfiner>().m_ConfineScreenEdges = false;
        SetTime(0.025f);
        while (FindObjectOfType<MelonPowerupController>().currentOres != 0)
        {
            yield return new WaitForSeconds(0.000000000001f);
        }
        SetTime(1f);
        FindObjectOfType<PostProcessingManager>().stopSlowEffect = true;
        FindObjectOfType<CinemachineConfiner>().m_ConfineScreenEdges = true;
        yield return new WaitForSeconds(0.25f);
        SC(dialogues[2]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
