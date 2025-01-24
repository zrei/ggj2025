using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [field: SerializeField]
    private float DirtyMovementSpeed { get; set; }
    [field: SerializeField]
    private float NeutralMovementSpeed { get; set; }
    [field: SerializeField]
    private float CleanMovementSpeed { get; set; }

    [SerializeField] private float m_InitialSlideForce;
    [field: SerializeField] 
    private float CleanDragAmount { get; set; }
    [field: SerializeField]
    private float NeutralDragAmount { get; set; }
    [field: SerializeField]
    private float DirtyDragAmount { get; set; }
    [Tooltip("When velocity falls below this threshold, the slide state is exited")]
    [SerializeField] private float m_StopDragThreshold = 0.1f;

    private Rigidbody2D m_Rigidbody;
    private SpriteRenderer m_SpriteRenderer;

    private Vector2 m_MovementVector;

    #region Tile State
    private TileType m_CurrTileType = TileType.NEUTRAL;
    private TileType CurrTileType
    {
        get
        {
            return m_CurrTileType;
        }
        set
        {
            if (m_CurrTileType != value)
            {
                // TODO: do whatever you need on tile type change
                if (m_IsSliding)
                    m_Rigidbody.drag = GetDrag();
            }
            m_CurrTileType = value;
        }
    }
    #endregion

    #region Slide
    private const float NORMAL_MOVEMENT_DRAG = 0;
    private float m_StopDragThresholdSquared;
    private bool m_IsSliding = false;
    private Vector2 m_CurrSlideDirection;
    private Vector2 m_PreviousVelocity;
    private Vector3 m_PreviousWorldPosition;
    #endregion

    private void Awake()
    {
        CurrTileType = TileType.NEUTRAL;
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Rigidbody.drag = NORMAL_MOVEMENT_DRAG;
        m_StopDragThresholdSquared = m_StopDragThreshold * m_StopDragThreshold;
    }

    private void Update()
    {
        if (m_IsSliding)
        {
            GridManager.Instance.SetTileStatus(m_PreviousWorldPosition, TileType.CLEAN);
        }

        CurrTileType = GridManager.Instance.GetTileTypeAtWorldCoordinates(transform.position);
        GetPlayerInputs();
        FlipSprite();
        UpdateAim();
        UpdateStateValues();
    }

    private void UpdateStateValues()
    {
        m_PreviousVelocity = m_Rigidbody.velocity;
        m_PreviousWorldPosition = m_Rigidbody.position;
    }

    private void UpdateAim()
    {
        m_CurrSlideDirection = -(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
    }

    private void GetPlayerInputs()
    {
        if (!m_IsSliding && Input.GetMouseButtonDown(0))
        {
            m_Rigidbody.velocity = Vector2.zero;
            m_Rigidbody.drag = GetDrag();
            m_Rigidbody.AddForce(m_CurrSlideDirection.normalized * m_InitialSlideForce);
            m_IsSliding = true;
            GlobalEvents.Player.OnPlayerStartSliding?.Invoke();
        }
        else if (m_IsSliding && m_Rigidbody.velocity.sqrMagnitude < m_PreviousVelocity.sqrMagnitude && m_Rigidbody.velocity.sqrMagnitude < m_StopDragThresholdSquared)
        {
            // exit slide
            m_IsSliding = false;
            m_Rigidbody.drag = NORMAL_MOVEMENT_DRAG;
            GlobalEvents.Player.OnPlayerStopSliding?.Invoke();
        }
 
        m_MovementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void FlipSprite()
    {
        float x = m_Rigidbody.velocity.x;

        // TODO: Change depending on the movement of sprite
        if (x > 0)
        {

        }
        else if (x < 0)
        {

        }
    }

    private void FixedUpdate()
    {
        if (!m_IsSliding)
            m_Rigidbody.velocity = m_MovementVector.normalized * GetMovementSpeed();
    }

    private float GetMovementSpeed()
    {
        return CurrTileType switch
        {
            TileType.CLEAN => CleanMovementSpeed,
            TileType.DIRTY => DirtyMovementSpeed,
            TileType.NEUTRAL => NeutralMovementSpeed,
            _ => 0,
        };
    }

    private float GetDrag()
    {
        return CurrTileType switch
        {
            TileType.CLEAN => CleanDragAmount,
            TileType.DIRTY => DirtyDragAmount,
            TileType.NEUTRAL => NeutralDragAmount,
            _ => 0,
        };
    }
}
