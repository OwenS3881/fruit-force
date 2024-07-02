using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonSpawnDetector : MonoBehaviour
{

    private Transform wall;
    private Vector3 wallStartPos;
    private bool active;
    private bool detectingPlayer;
    private float playerTime;
    public float spawnTime;
    public float wallSpeed;
    private bool movingPlayer;

    // Start is called before the first frame update
    void Start()
    {
        wall = transform.GetChild(0).transform;
        wallStartPos = wall.transform.localPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            detectingPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            detectingPlayer = false;
        }
    }

    IEnumerator MovePlayer()
    {
        while (detectingPlayer)
        {
            wall.position = new Vector3(wall.position.x + (wallSpeed * Time.deltaTime * transform.localScale.x), wall.position.y, wall.position.z);
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        wall.localPosition = wallStartPos;
        movingPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            active = FindObjectOfType<PhotonGameManager>().gameStarted;
            return;
        }

        if (detectingPlayer)
        {
            playerTime += Time.deltaTime;
        }
        else
        {
            playerTime = 0;
        }

        if (!movingPlayer && playerTime > spawnTime)
        {
            movingPlayer = true;
            StartCoroutine(MovePlayer());
        }
    }
}
