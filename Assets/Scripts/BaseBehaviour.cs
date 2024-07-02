using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;

public class BaseBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public Timeline time
    {
        get
        {
            return GetComponent<Timeline>();
        }
    }
}
