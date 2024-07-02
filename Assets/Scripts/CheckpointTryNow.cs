using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Chronos;

public class CheckpointTryNow : BaseBehaviour
{

    public string character;
    public string sceneName;
    public float lifetime = 5f;
    private TMP_Text myText;
    private Animator anim;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(End(lifetime));      
    }

    IEnumerator End(float delay)
    {
        yield return time.WaitForSeconds(delay);
        anim.SetTrigger("Disappear");
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
        {
            yield return time.WaitForSeconds(0.02f);
        }
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        if (myText == null)
        {
            myText = GetComponentInChildren<TMP_Text>();
        }
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        myText.text = "Try " + character + " Now?";
        gameObject.SetActive(true);
        anim.SetTrigger("Appear");
    }

    public void TryNow()
    {
        FindObjectOfType<DataManager>().SaveData();
        FindObjectOfType<LevelLoader>().LoadSceneEffect(sceneName);
    }
}
