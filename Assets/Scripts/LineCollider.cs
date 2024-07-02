using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class LineCollider : MonoBehaviour
{

    private EdgeCollider2D edge;
    private LineRenderer line;

    private Vector3[] linePoints3;
    private Vector2[] linePoints2;

    // Start is called before the first frame update
    void Start()
    {
        edge = GetComponent<EdgeCollider2D>();
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        linePoints3 = new Vector3[line.positionCount];
        line.GetPositions(linePoints3);
        linePoints2 = new Vector2[linePoints3.Length];
        for (int i = 0; i < linePoints3.Length; i++)
        {
            linePoints2[i] = (Vector2)linePoints3[i];
        }
        edge.points = linePoints2;
    }
}
