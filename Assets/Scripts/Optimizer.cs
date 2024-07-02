using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Optimizer : MonoBehaviour
{
    private Ore[] ores;
    public float oreDistance = 20f;

    private Robot[] robots;
    public float robotDistance = 20f;

    private Drone[] drones;
    public float droneDistance = 20f;

    private Crawler[] crawlers;
    public float crawlerDistance = 20f;

    private Mushroom[] mushrooms;
    public float mushroomDistance = 20f;

    private MovingPlatformParent[] movingPlatforms;
    public float movingPlatformsDistance = 50f;

    private Ground[] grounds;
    public float groundsDistance = 150f;

    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        ores = FindObjectsOfType<Ore>();
        robots = FindObjectsOfType<Robot>();
        drones = FindObjectsOfType<Drone>();
        crawlers = FindObjectsOfType<Crawler>();
        mushrooms = FindObjectsOfType<Mushroom>();
        movingPlatforms = FindObjectsOfType<MovingPlatformParent>();
        grounds = FindObjectsOfType<Ground>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ores.Length; i++)
        {
            if (ores[i] != null)
            {
                ores[i].gameObject.SetActive(Vector2.Distance(transform.position, ores[i].transform.position) <= oreDistance);
            }
        }

        for (int i = 0; i < robots.Length; i++)
        {
            if (robots[i] != null)
            {
                robots[i].gameObject.SetActive(Vector2.Distance(transform.position, robots[i].transform.position) <= robotDistance);
            }
        }

        for (int i = 0; i < drones.Length; i++)
        {
            if (drones[i] != null)
            {
                drones[i].gameObject.SetActive(Vector2.Distance(transform.position, drones[i].transform.position) <= droneDistance);
            }
        }

        for (int i = 0; i < crawlers.Length; i++)
        {
             
            if (crawlers[i] != null)
            {
                if (crawlers[i].GetComponentInChildren<TrailRenderer>() != null)
                {
                    continue;
                }
                crawlers[i].gameObject.SetActive(Vector2.Distance(transform.position, crawlers[i].transform.position) <= crawlerDistance);
            }
        }

        for (int i = 0; i < mushrooms.Length; i++)
        {
            if (mushrooms[i] != null)
            {
                mushrooms[i].gameObject.SetActive(Vector2.Distance(transform.position, mushrooms[i].transform.position) <= mushroomDistance);
            }
        }

        for (int i = 0; i < movingPlatforms.Length; i++)
        {
            
            if (movingPlatforms[i] != null)
            {
                if (movingPlatforms[i].GetComponentInChildren<StickyPlatform>() != null)
                {
                    continue;
                }
                movingPlatforms[i].gameObject.SetActive(Vector2.Distance(transform.position, movingPlatforms[i].transform.position) <= movingPlatformsDistance);
            }
        }

        for (int i = 0; i < grounds.Length; i++)
        {

            if (grounds[i] != null)
            {
                if (!grounds[i].optimized)
                {
                    continue;
                }
                grounds[i].gameObject.SetActive(Vector2.Distance(transform.position, grounds[i].transform.position) <= groundsDistance);
            }
        }

        text.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }
}
