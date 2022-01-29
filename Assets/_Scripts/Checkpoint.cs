using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Checkpoint : MonoBehaviour
{
    public Vector3 Position => transform.position;

    public bool Active => m_Active;

    public static event Action<Checkpoint> OnActivate;

    [SerializeField] private Collider2D m_Trigger;
    [SerializeField] private bool m_Active;
    [SerializeField] private Animator m_AC;

    private void Start() 
    {
        m_Trigger.enabled = !m_Active;
        m_AC.SetBool("Active", Active);
    }

    private void OnEnable() 
    {
        OnActivate += CheckpointActivated;
    }

    private void OnDisable() 
    {
        OnActivate -= CheckpointActivated;
    }

    private void CheckpointActivated(Checkpoint other)
    {
        gameObject.SetActive(this == other || !m_Active);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        OnActivate?.Invoke(this);
        m_Active = true;
        m_Trigger.enabled = false;
    }

    private void OnValidate() 
    {
        m_Trigger.isTrigger = true;
    }

    private void Reset() 
    {
        m_Trigger = GetComponent<Collider2D>();
        m_Trigger.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Checkpoint");
        gameObject.tag = "Checkpoint";
    }
}
