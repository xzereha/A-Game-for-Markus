using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowPlayerX : MonoBehaviour
{
    [SerializeField] private float m_DampTime = 0.5f;
    
    [SerializeField] private Transform m_Target;
    [SerializeField] private Camera m_Camera;

    private Vector3 m_CurrentVelocity = Vector3.zero;

    private void Update()
    {
        Vector3 destination = m_Target.position;
        destination.z = transform.position.z;
        Vector3 final = Vector3.SmoothDamp(transform.position, destination, ref m_CurrentVelocity, m_DampTime);

        transform.position = final;
    }

    private void Reset() 
    {
        m_Camera = GetComponent<Camera>();
        m_Target = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
