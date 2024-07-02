using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ParallaxC : MonoBehaviour
{
    public Transform cam;
    private Vector3 lastCameraPos;
    public Vector2 effectMultiplier;
    private float textureUnitSizeX;
    private float textureUnitSizeY;
    public bool vertciallyRepeat;

    void Start()
    {
        lastCameraPos = cam.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.localScale.x;
        textureUnitSizeY = (texture.height / sprite.pixelsPerUnit)  * transform.localScale.y;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cam.position - lastCameraPos;
        transform.position += new Vector3 (deltaMovement.x * effectMultiplier.x, deltaMovement.y * effectMultiplier.y, 0f);
        lastCameraPos = cam.position;

        if (Mathf.Abs(cam.position.x - transform.position.x) >= textureUnitSizeX)
        {
            
            float offsetPosX = ((cam.position.x - transform.position.x)) % (textureUnitSizeX);
            transform.position = new Vector3(cam.position.x + offsetPosX, transform.position.y);
        }

        if (Mathf.Abs(cam.position.y - transform.position.y) >= textureUnitSizeY && vertciallyRepeat)
        {
            float offsetPosY = ((cam.position.y - transform.position.y)) % (textureUnitSizeY);
            transform.position = new Vector3(transform.position.x, cam.position.y + offsetPosY);
        }
    }
}
