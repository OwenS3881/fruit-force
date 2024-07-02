using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using DialogueEditor;
using UnityEngine.UI;

public class BananaDialogue : MonoBehaviour
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
        while (!FindObjectOfType<BananaPowerupController>().GetComponent<Toggle>().isOn)
        {
            yield return new WaitForSeconds(0.000000000001f);
        }
        EC();
        SC(dialogues[1]);
        while(FindObjectOfType<BananaProjectile>() == null)
        {
            yield return new WaitForSeconds(0.000000000001f);
        }
        yield return new WaitForSeconds(0.1f);
        SetTime(0f);
        SC(dialogues[2]);
        while (GameObject.FindWithTag("PointJoystick").GetComponent<FixedJoystick>().Direction.sqrMagnitude == 0)
        {
            yield return new WaitForSeconds(0.000000000001f);
        }
        EC();
        Timekeeper.instance.Clock("Banana").localTimeScale = 1f;
        Timekeeper.instance.Clock("DeathEffects").localTimeScale = 1f;
        while (FindObjectOfType<Robot>() != null)
        {
            yield return new WaitForSeconds(0.000000000001f);
        }
        SC(dialogues[3]);

    }

}
