using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedPanel : MonoBehaviour
{
    public bool locked = false;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Locked", locked);
    }
}
