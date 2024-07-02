using System.Collections;
using UnityEngine;
using DialogueEditor;

public class DragonfruitDialogue : MonoBehaviour
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

    public void ReturnToChar()
    {
        FindObjectOfType<LevelLoader>().LoadSceneEffect("CharacterSelect");
    }

    IEnumerator Dialogue()
    {
        SC(dialogues[0]);
        yield return new WaitForSeconds(0.1f);
        while (FindObjectOfType<DragonfruitPowerupController>().currentOres != 0)
        {
            yield return new WaitForSeconds(0.000000000001f);
        }
        EC();
        yield return new WaitForSeconds(0.25f);
        SC(dialogues[1]);
    }
}
