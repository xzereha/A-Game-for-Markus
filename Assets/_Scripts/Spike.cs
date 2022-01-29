using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Spike : MonoBehaviour
{
    [SerializeField] private SpriteData m_SpriteMap;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        MessageHandler.TriggerEvent("KillPlayer");
    }
    
    [ContextMenu("Randomize Sprite")]
    private void RandomizeSprite()
    {
        if(m_SpriteMap == null) Debug.LogAssertion("No sprite data found");
        SpriteData spriteData = m_SpriteMap;
        int sprites = spriteData.SpikeSprits.Length;
        int sprite = Random.Range(0, sprites);
        GetComponent<SpriteRenderer>().sprite = spriteData.SpikeSprits[sprite];
    }

    private void Reset() 
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1.0f, 0.6f);
        collider.offset = new Vector2(0.0f, 0.3f);
        gameObject.layer = LayerMask.NameToLayer("Death");
        gameObject.name = "Spike";
        m_SpriteMap ??= Resources.FindObjectsOfTypeAll<SpriteData>()[0]; 
        RandomizeSprite();
    }
}
