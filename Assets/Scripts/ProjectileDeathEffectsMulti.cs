using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileDeathEffectsMulti : MonoBehaviour
{
    public PhotonView view;
    public Color goodStartColor, badStartColor;
    public ParticleSystem.MinMaxGradient goodGradient, badGradient;
    public ParticleSystem ps;

    private void Awake()
    {
        ParticleSystem.ColorOverLifetimeModule pscl = ps.colorOverLifetime;
        ParticleSystem.MainModule m = ps.main;
        if (view.IsMine)
        {
            m.startColor = goodStartColor;
            pscl.color = goodGradient;
        }
        else
        {
            m.startColor = badStartColor;
            pscl.color = badGradient;
        }
    }
}
