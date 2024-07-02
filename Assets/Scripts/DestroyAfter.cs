using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyAfter : BaseBehaviour
{

    [SerializeField] private float destroyTime = 0f;
    public bool isPhoton = false;

    // Update is called once per frame
    void Update()
    {
      StartCoroutine(destroy());
    }

    IEnumerator destroy()
    {
        if (time != null)
        {
            yield return time.WaitForSeconds(destroyTime);
        }
        else
        {
            yield return new WaitForSeconds(destroyTime);
        }
        if (!isPhoton)
        {
            Destroy(gameObject);
        }
        else if (gameObject.GetPhotonView().IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
