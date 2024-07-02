using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PhotonPowerShield : MonoBehaviour
{

    public Light2D shieldLight;
    public Collider2D shieldCollider;
    private bool active;

    // Update is called once per frame
    void Update()
    {
        active = transform.localScale.z != 1;
        shieldLight.enabled = active;
        shieldCollider.enabled = active;
    }
}
