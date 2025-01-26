using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [field: SerializeField]
    private float DirtyMovementSpeed { get; set; }
    [field: SerializeField]
    private float NeutralMovementSpeed { get; set; }
    [field: SerializeField]
    private float CleanMovementSpeed { get; set; }

    private Rigidbody2D m_Rigidbody;
    private SpriteRenderer m_SpriteRenderer;

    private Vector2 m_MovementVector;

    #region TileType
    private TileType m_CurrTileType = TileType.NEUTRAL;
    private TileType CurrTileType
    {
        get
        {
            return m_CurrTileType;
        }
        set
        {

            m_CurrTileType = value;
        }
    }
    #endregion

    #region Slide
    private const float NORMAL_MOVEMENT_DRAG = 0;
    #endregion

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_Rigidbody.drag = NORMAL_MOVEMENT_DRAG;
    }

    private void Update()
    {
        if (BattleManager.Instance.State == GameState.BetweenWaves)
            return;

        GetPlayerInputs();
        FlipSprite();
    }

    private void GetPlayerInputs()
    { 
        m_MovementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void FlipSprite()
    {
        float x = m_Rigidbody.velocity.x;

        if (PlayerBeam.Instance.IsSliding)
        {
            m_SpriteRenderer.flipX = false;
            var dir = PlayerBeam.Instance.CurrentSlideDirection;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float finalAngle = targetAngle + 180f;
            transform.rotation = Quaternion.Euler(0, 0, finalAngle);
        }
        else
        {
            transform.rotation = Quaternion.identity;

            if (x > 0)
            {
                m_SpriteRenderer.flipX = true;
            }
            else if (x < 0)
            {
                m_SpriteRenderer.flipX = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (BattleManager.Instance.State == GameState.BetweenWaves)
        {
            return;
        }

        if (CurrTileType == TileType.CLEAN)
        {
            m_Rigidbody.AddForce(m_MovementVector * 5f, ForceMode2D.Force);
        }

        if (!PlayerBeam.Instance.IsSliding)
        {
            m_Rigidbody.velocity = m_MovementVector.normalized * GetMovementSpeed();
            GlobalEvents.Player.OnPlayerMove?.Invoke();
        }
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
}
