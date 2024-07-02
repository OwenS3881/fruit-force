using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Experimental.Rendering.Universal;

public class MuzzleMulti : MonoBehaviour
{
    public PhotonView view;
    public Light2D light;
    public Color good, bad;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (view.IsMine)
        {
            light.color = good;
        }
        else
        {
            light.color = bad;
        }
    }

    public void Burst()
    {
        StartCoroutine(CoBurst());
    }

    IEnumerator CoBurst()
    {
        anim.SetBool("Burst", true);
        yield return null;
        anim.SetBool("Burst", false);
    }
}
