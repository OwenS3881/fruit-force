using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandTrialsManager : MonoBehaviour
{
    public List<IslandData> islands = new List<IslandData>();

    IEnumerator IslandCompleteSound()
    {
        for (int i = 0; i < 4; i++)
        {
            AudioManager.instance.PlaySoundOneShot("IslandTrial");
            yield return new WaitForSeconds(0.65f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (IslandData island in islands)
        {
            if (island.complete)
            {
                StartCoroutine(IslandCompleteSound());
                islands.Remove(island);
            }
        }
        foreach (IslandData island in islands)
        {
            if (!island.complete)
            {
                island.laserDoor.SetActive(island.trigger.triggered);
            }
            for (int i = 0; i < island.targets.Count; i++)
            {
                if (island.targets[i] == null)
                {
                    island.targets.Remove(island.targets[i]);
                }
            }
            if (island.targets.Count <= 0 && !island.complete)
            {
                island.laserArena.GetComponent<Laser>().Fade();
                island.laserDoor.GetComponent<Laser>().Fade();
                island.complete = true;
            }
        }
    }
}
