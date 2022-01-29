using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class TriggerEvent : MonoBehaviour
{
    [Tooltip("Should the trigger send events more than once")]
    [SerializeField] private bool m_RepeatTrigger = true;
    [SerializeField] private bool m_EndOfLevel;
    public UnityEvent OnTrigger;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag != "Player") return;
        Debug.Log($"{name} triggered by {other.gameObject.name}");
        if(m_EndOfLevel)
        {
            GameManager.LoadNextLevel();
        }
        else
        {
            OnTrigger.Invoke();
            GetComponent<Collider2D>().enabled = m_RepeatTrigger;
        }
    }

    private void Reset() 
    {
        gameObject.layer = LayerMask.NameToLayer("Trigger");
        GetComponent<Collider2D>().isTrigger = true;
    }
}
