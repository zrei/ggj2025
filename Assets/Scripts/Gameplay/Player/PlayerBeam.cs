using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerBeam : Singleton<PlayerBeam>
{
    public float SpeedMagnitude => m_Rigidbody.velocity.magnitude;
    public bool IsSliding => m_IsSliding;

    [field: SerializeField]
    private float BeamTime;

    [field: SerializeField]
    private float BeamCooldown {  get; set; }

    [field: SerializeField]
    private float SlideForce { get; set; }

    [field: SerializeField, Header("Drag Amounts")]
    private float CleanDragAmount { get; set; }

    [field: SerializeField]
    private float NeutralDragAmount { get; set; }

    [field: SerializeField]
    private float DirtyDragAmount { get; set; }

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

            if (!m_IsSliding)
            {
                GlobalEvents.Player.OnPlayerMoveOntoTile?.Invoke(value == TileType.DIRTY);
            }

            m_CurrTileType = value;
        }
    }
    #endregion

    private Rigidbody2D m_Rigidbody;
    private Vector2 m_CurrentSlideDirection;
    private float m_BeamCooldownTimer;
    private float m_BeamTimer;
    private bool m_IsSliding;
    private const float NORMAL_MOVEMENT_DRAG = 0;

    protected override void HandleAwake()
    {
        m_CurrTileType = TileType.NEUTRAL;
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_BeamTimer = BeamCooldown;
    }

    private void Update()
    {
        GetPlayerInputs();
        UpdateAim();

        if (m_BeamCooldownTimer > 0)
        {
            m_BeamCooldownTimer -= Time.deltaTime;
        }

        if (m_IsSliding)
        {
            if (m_BeamTimer > 0)
            {
                m_BeamTimer -= Time.deltaTime;
            }
            else
            {
                ExitSlide();
            }
        }
    }

    private void GetPlayerInputs()
    {
        CurrTileType = GridManager.Instance.GetTileTypeAtWorldCoordinates(transform.position);

        if (TutorialManager.Instance != null && !TutorialManager.Instance.CanPlayerShoot)
        {
            return;
        }

        if (Player.Instance.IsDead)
        {
            return;
        }

        if (m_BeamCooldownTimer > 0)
        {
            return;
        }

        if (BattleManager.Instance.State == GameState.BetweenWaves)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            if (!m_IsSliding)
            {
                m_BeamTimer = BeamTime;
                m_IsSliding = true;
                GlobalEvents.Player.OnPlayerShoot?.Invoke();
                return;
            }
            else
            {
                m_Rigidbody.AddForce(m_CurrentSlideDirection * SlideForce);
                m_Rigidbody.drag = GetDrag();
                if (m_IsSliding)
                {
                    GridManager.Instance.SetTileStatus(transform.position, TileType.CLEAN);
                }
                GlobalEvents.Player.OnPlayerMoveOntoTile?.Invoke(false);
            } 
        }
        else if (m_BeamTimer > 0 && m_IsSliding && Input.GetMouseButtonUp(0))
        {
            ExitSlide();
        }
    }

    private void UpdateAim()
    {
        m_CurrentSlideDirection = -(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
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

    private void ExitSlide()
    {
        m_IsSliding = false;
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.drag = NORMAL_MOVEMENT_DRAG;
        m_BeamCooldownTimer = BeamCooldown;
        GlobalEvents.Player.OnPlayerStopSliding?.Invoke();
    }
}
