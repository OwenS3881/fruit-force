using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateIndicatorScript : MonoBehaviour
{

  LineRenderer line;
  bool rotating = false;
  public BlasterPoint BlasterPointScript;
  public GameObject player;
  [SerializeField] private float forceMultiplier;

  void Start()
  {
    line = GetComponent<LineRenderer>();
  }

  IEnumerator Rotate()
  {
    yield return new WaitForSeconds(0.0000000000001f);
    Touch firstTouch = Input.GetTouch(0);
    Vector3 firstTouchPos = Camera.main.ScreenToWorldPoint(firstTouch.position);
    firstTouchPos.z = 0f;
    Vector3 newTouchPos = firstTouchPos;
    while (Input.touchCount > 0)
    {
      line.SetPosition(0, firstTouchPos);
      newTouchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
      newTouchPos.z = 0f;
      line.SetPosition(1, newTouchPos);
      BlasterPointScript.PointBlaster(firstTouchPos, newTouchPos);
      yield return new WaitForSeconds(0.0000000000001f);
    }
    Vector2 force = firstTouchPos - newTouchPos;
    player.GetComponent<Rigidbody2D>().AddForce(force*forceMultiplier, ForceMode2D.Impulse);
    line.SetPosition(0, Vector3.zero);
    line.SetPosition(1, Vector3.zero);
    rotating = false;
  }

    // Update is called once per frame
    void Update()
    {
      if (Input.touchCount > 0 && !rotating)
      {
        StartCoroutine(Rotate());
        rotating = true;
      }
    }
}
