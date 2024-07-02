using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;

public class MovingPlatformParent : BaseBehaviour
{

    public GameObject platform;
    public GameObject pointsParent;
    private GameObject[] points;
    private int currentIndex = 0;
    private int nextIndex = 1;
    private float tLerp = 0f;
    [Range(0f,0.1f)]
    public float speed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        points = new GameObject[pointsParent.transform.childCount];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = pointsParent.transform.GetChild(i).gameObject;
        }
        platform.transform.position = points[currentIndex].transform.position;
        if (platform.GetComponent<SpriteRenderer>() != null)
        {
            platform.GetComponent<SpriteRenderer>().enabled = false;
        }

        foreach (GameObject p in points)
        {
            if (p.GetComponent<SpriteRenderer>() != null)
            {
                p.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    void ReachedPoint()
    {
        tLerp = 0;
        currentIndex++;
        nextIndex++;
        if (currentIndex > points.Length-1)
        {
            currentIndex = 0;
        }
        if (nextIndex > points.Length - 1)
        {
            nextIndex = 0;
        }
        platform.transform.position = points[currentIndex].transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Timekeeper.instance.Clock("Root").localTimeScale > 0)
        {
            platform.transform.position = Vector3.Lerp(points[currentIndex].transform.position, points[nextIndex].transform.position, tLerp);
            tLerp += speed;
            tLerp = Mathf.Clamp(tLerp, 0, 1);
            if (tLerp >= 1)
            {
                ReachedPoint();
            }
        }
    }
}
