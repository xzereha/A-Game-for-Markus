using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, PlayerInput.IGameplayActions
{
    public bool Grounded => m_Grounded;
    public float XVelocity => m_InputDirection;
    public float YVelocity => m_YVelocity;

    [Tooltip("The player rigidbody")]
    [SerializeField] private Rigidbody2D m_RigidBody;

    [Tooltip("The player collider")]
    [SerializeField] private Collider2D m_Collider;

    [Tooltip("Box collider for detecting if the player is grounded or not")]
    [SerializeField] private BoxCollider2D m_GroundCheckCollider;

    [Tooltip("What collision layers should count as ground")]
    [SerializeField] private LayerMask m_CollisionLayer;

    [Header("Jumping")]
    [Tooltip("How high is the peak of the jump")]
    [SerializeField] private float m_JumpHeight = 2.0f;

    [Tooltip("How long does it take to reach the peak of the jump")]
    [SerializeField] private float m_TimeToApex = 1.0f;
    
    [Tooltip("How much what is the modifier for the falling speed")]
    [SerializeField] private float m_FallingModifier = 1.0f;

    [Header("Movement")]
    [Tooltip("Player movement speed in units per second")]
    [SerializeField] private float m_MovementSpeed = 2.0f;

    [Tooltip("Time to reach maximum movement speed")]
    [SerializeField] private float m_AccelerationTime = 0.1f;

    private PlayerInput m_Controls; //!< The object for the input hangler
    private float m_JumpVelocity; //!< The velocity constant for the jump
    private float m_Gravity; //!< The gravity constant for the jump

    private float m_InputDirection; //!< The direction the player is moving
    private bool m_Grounded; //!< If the player is currently touching the ground
    private bool m_Jumping; //!< If the player is still going upwards
    private float m_XVelocity; //!< Current X velocity of the player
    private float m_YVelocity; //!< Current Y velocity of the player

    public void Kill()
    {
        transform.position = WorldManager.CurrentCheckpoint;
        m_InputDirection = 0;
        m_Grounded = false;
        m_Jumping = false;
        m_XVelocity = 0;
        m_YVelocity = -1.0f;
    }

    private void Awake() 
    {
        m_Controls = new PlayerInput();
        m_Controls.Gameplay.SetCallbacks(this);
    }

    private void Start()
    {
        CalulateJumpValues();
    }

    private void OnEnable() 
    {
        m_Controls.Gameplay.Enable();
        MessageHandler.StartListen("KillPlayer", Kill);
    }

    private void OnDisable() 
    {
        MessageHandler.StopListening("KillPlayer", Kill);
        m_Controls.Gameplay.Disable();
    }

    private void FixedUpdate() 
    {
        // Ground Check
        RaycastHit2D hit = Physics2D.BoxCast(m_GroundCheckCollider.transform.position + (Vector3)m_GroundCheckCollider.offset, m_GroundCheckCollider.size, 0, Vector2.down, 0.0f, m_CollisionLayer);
        m_Grounded = hit.collider != null;

        // Jump handling
        m_Jumping = m_Jumping && m_YVelocity > 0.0f;
        m_YVelocity = m_Grounded? Mathf.Max(m_YVelocity, 0) : m_YVelocity + m_Gravity * (m_Jumping? 1.0f : m_FallingModifier)  * Time.deltaTime;

        // Movement handling
        float acceleration = m_MovementSpeed / m_AccelerationTime;
        m_XVelocity += acceleration * m_InputDirection * Time.deltaTime;
        m_XVelocity = Mathf.Clamp(m_XVelocity, -m_MovementSpeed, m_MovementSpeed);

        // Final movement
        transform.Translate(m_XVelocity * Time.deltaTime, m_YVelocity * Time.deltaTime, 0);
    }

#region Helpers
    private void CalulateJumpValues()
    {
        m_JumpVelocity = (2 * m_JumpHeight) / m_TimeToApex;
        m_Gravity = (-2.0f * m_JumpHeight) / (m_TimeToApex * m_TimeToApex);
    }
#endregion

#region Input Callbacks
    public void OnMovement(InputAction.CallbackContext ctx)
    {
        float dir = ctx.ReadValue<float>();
        m_InputDirection = dir;
        m_XVelocity = dir == 0? 0 : m_XVelocity;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        bool started = ctx.started && m_Grounded;
        m_YVelocity = started? m_JumpVelocity : m_YVelocity;
        m_Jumping = ctx.performed && m_YVelocity > 0.0f;
    }

    public void OnWorldSwitch(InputAction.CallbackContext ctx)
    {

    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        MessageHandler.TriggerEvent("PauseGame");
    }
#endregion

#region Editor

    private void OnValidate() 
    {
        // Update jump height incase the variable was changed
        CalulateJumpValues();
        m_AccelerationTime = Mathf.Max(m_AccelerationTime, 0.001f);
    }

    private void Reset() 
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Collider = GetComponent<Collider2D>();
        m_CollisionLayer = 64;
    }
#endregion


    // TODO!!!!!!!!! REMOVE!!!!!
    private void OnGUI() 
    {
        if(GUI.Button(new Rect(0, 60, 100, 40), "Die"))
        {
            Kill();
        }
    }
}
