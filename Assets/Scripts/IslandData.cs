using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IslandData
{
    public List<GameObject> targets = new List<GameObject>();
    public GameObject laserDoor;
    public GameObject laserArena;
    public PlayerTrigger trigger;
    public bool complete = false;
}
