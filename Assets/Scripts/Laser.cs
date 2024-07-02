using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public GameObject[] vfxs;
    private LineRenderer line;
    private bool fading = false;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        vfxs[0].SetActive(true);
        vfxs[1].SetActive(true);
    }

    public void Fade()
    {
        if (!fading)
        {
            fading = true;
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        float t = 1;
        while(line.material.color.a > 0)
        {
            Color c = line.material.color;
            c.a = t;
            line.material.SetColor("_Color", c);
            t -= 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        vfxs[0].transform.position = line.GetPosition(0);
        vfxs[1].transform.position = line.GetPosition(line.positionCount - 1);

        vfxs[0].transform.right = line.GetPosition(1) - line.GetPosition(0);
        vfxs[1].transform.right = line.GetPosition(line.positionCount - 1) - line.GetPosition(line.positionCount - 2);
    }
}
