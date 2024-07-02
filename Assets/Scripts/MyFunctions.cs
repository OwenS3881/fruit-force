using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public static class MyFunctions 
{
    public static void SetPosition(Transform obj, int posIndex, float value)
    {
        float[] newPos = new float[3];
        for (int i = 0; i < newPos.Length; i++)
        {
            if (i == posIndex)
            {
                newPos[i] = value;
            }
            else if (i == 0)
            {
                newPos[i] = obj.position.x;
            }
            else if (i == 1)
            {
                newPos[i] = obj.position.y;
            }
            else if (i == 2)
            {
                newPos[i] = obj.position.z;
            }
        }
        obj.position = new Vector3(newPos[0], newPos[1], newPos[2]);
    }

    public static void SetScale(Transform obj, int scaleIndex, float value)
    {
        float[] newScale = new float[3];
        for (int i = 0; i < newScale.Length; i++)
        {
            if (i == scaleIndex)
            {
                newScale[i] = value;
            }
            else if (i == 0)
            {
                newScale[i] = obj.localScale.x;
            }
            else if (i == 1)
            {
                newScale[i] = obj.localScale.y;
            }
            else if (i == 2)
            {
                newScale[i] = obj.localScale.z;
            }
        }
        obj.localScale = new Vector3(newScale[0], newScale[1], newScale[2]);
    }

    public static void SetRotation(Transform obj, int rotateIndex, float value)
    {
        float[] newRotation = new float[3];
        for (int i = 0; i < newRotation.Length; i++)
        {
            if (i == rotateIndex)
            {
                newRotation[i] = value;
            }
            else if (i == 0)
            {
                newRotation[i] = obj.rotation.x;
            }
            else if (i == 1)
            {
                newRotation[i] = obj.rotation.y;
            }
            else if (i == 2)
            {
                newRotation[i] = obj.rotation.z;
            }
        }
        obj.rotation = Quaternion.Euler(newRotation[0], newRotation[1], newRotation[2]);
    }

    public static float VectorToFloat(Vector2 direction)
    {
        double dX = direction.x;
        double dY = direction.y;
        double newDirection = Math.Atan2(dY, dX);
        newDirection = newDirection * (180 / Math.PI);
        float floatNewDirection = (float)newDirection;
        return floatNewDirection;
    }

    /*
    public static void SmoothRotate(Transform obj, int rotateIndex, float value, float speed)
    {
        StartCoroutine(Sr)
    }

    IEnumerator SR(Transform obj, int rotateIndex, float value, float speed)
    {

    }
    */

}
