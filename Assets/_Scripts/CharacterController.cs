using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class CharacterController : MonoBehaviour//, PlayerControls.IGameplayActions
{
    [SerializeField] private Rigidbody2D m_RigidBody;
    [SerializeField] private Collider2D m_Collider;
    [SerializeField] private BoxCollider2D m_GroundCheckCollider;
    [SerializeField] private LayerMask m_CollisionLayer;

    [Header("Jumping")]
    [Tooltip("How high is the peak of the jump")]
    [SerializeField] private float m_JumpHeight = 2.0f;
    [Tooltip("How long does it take to reach the peak of the jump")]
    [SerializeField] private float m_TimeToApex = 1.0f;
    [Tooltip("How much what is the modifier for the falling speed")]
    [SerializeField] private float m_FallingModifier = 1.0f;

    [Header("Movement")]
    [SerializeField] private float m_MovementSpeed = 2.0f;

    private PlayerControls m_Controls;
    private float m_JumpVelocity;
    private float m_Gravity;
    private float m_YVelocity;
    private float m_InputDirection;
    private bool m_Grounded;

    private void Start()
    {
        m_JumpVelocity = (2 * m_JumpHeight) / m_TimeToApex;
        m_Gravity = (-2.0f * m_JumpHeight) / (m_TimeToApex * m_TimeToApex);
    }

    //private void OnEnable() 
    //{
    //    if(m_Controls == null)
    //    {
    //        m_Controls = new PlayerControls();
    //        m_Controls.Gameplay.SetCallbacks(this);
    //    }
    //    m_Controls.Gameplay.Enable();
    //}

    //private void OnDisable() 
    //{
    //    m_Controls.Gameplay.Disable();
    //}

    private void FixedUpdate() 
    {
        RaycastHit2D hit = Physics2D.BoxCast(m_GroundCheckCollider.transform.position + (Vector3)m_GroundCheckCollider.offset, m_GroundCheckCollider.size, 0, Vector2.down, 0.0f, m_CollisionLayer);
        m_Grounded = hit.collider? true : false;
        m_YVelocity = m_Grounded? Mathf.Max(m_YVelocity, 0) : m_YVelocity + m_Gravity * Time.deltaTime;
        transform.Translate(new Vector3(m_InputDirection * m_MovementSpeed * Time.deltaTime, m_YVelocity * Time.deltaTime, 0));
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        float dir = ctx.ReadValue<float>();
        m_InputDirection = dir;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        bool started = ctx.started && m_Grounded;
        m_YVelocity = started? m_JumpVelocity : m_YVelocity;
    }

    private void OnValidate() 
    {
        // Update jump height incase the variable was changed
        m_JumpVelocity = (2 * m_JumpHeight) / m_TimeToApex;
        m_Gravity = (-2.0f * m_JumpHeight) / (m_TimeToApex * m_TimeToApex);
    }

    private void Reset() 
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();
        m_CollisionLayer = 64;
    }
}
