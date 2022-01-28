using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class CharacterController : MonoBehaviour
{
    [SerializeField] private LayerMask m_CollisionLayer;
    [SerializeField] private float m_JumpHeight = 2.0f;
    [SerializeField] private float m_TimeToApex = 1.0f;
    [SerializeField] private float m_MovementSpeed = 2.0f;
    [SerializeField] private Rigidbody2D m_RigidBody;
    private float m_JumpVelocity;
    private float m_InputDirection;
    private bool m_Grounded;

    private void Start()
    {
        m_JumpVelocity = Mathf.Sqrt(-2.0f * Physics2D.gravity.y * m_JumpHeight);
    }

    private void Update()
    {

    }

    private void FixedUpdate() 
    {
        transform.Translate(new Vector3(m_InputDirection * m_MovementSpeed * Time.deltaTime, 0, 0));
        
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale * 0.9f, 0, -Vector2.up, 0.1f, m_CollisionLayer);
        m_Grounded = hit.collider? true : false;
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        float dir = ctx.ReadValue<float>();
        m_InputDirection = dir;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        bool started = ctx.started;
        if(!started || !m_Grounded) return;
        m_RigidBody.velocity = new Vector2(0, m_JumpVelocity);
    }

    public void OnControlsChanged(PlayerInput input)
    {
        string controller = input.currentControlScheme;
        Debug.Log($"Controller changed to {controller}");
    }

    private void OnValidate() 
    {
        // Update jump height incase the variable was changed
        m_JumpVelocity = Mathf.Sqrt(-2.0f * Physics2D.gravity.y * m_JumpHeight);
    }

    private void Reset() 
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_CollisionLayer = 64;
    }
}
