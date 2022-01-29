using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        MessageHandler.TriggerEvent("KillPlayer");
    }
    
    private void OnValidate() 
    {
        Reset();
    }

    private void Reset() 
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(0.9f, 0.6f);
        collider.offset = new Vector2(0.0f, 0.3f);
        gameObject.layer = LayerMask.NameToLayer("Death");
        gameObject.name = "Spike";
        SpriteData[] spriteDataArray = Resources.FindObjectsOfTypeAll<SpriteData>();
        if(spriteDataArray.Length == 0) return;
        SpriteData spriteData = spriteDataArray[0];
        int sprites = spriteData.SpikeSprits.Length;
        int sprite = Random.Range(0, sprites);
        GetComponent<SpriteRenderer>().sprite = spriteData.SpikeSprits[sprite];
    }
}
