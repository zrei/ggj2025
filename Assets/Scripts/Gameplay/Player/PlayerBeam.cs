using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class PlayerBeam : Singleton<PlayerBeam>
{
    public float SpeedMagnitude => m_Rigidbody.velocity.magnitude;
    public bool IsSliding => m_IsSliding;
    public Vector2 CurrentSlideDirection { get; private set; }

    [field: SerializeField, Header("Values")]
    private float BeamTime;

    [field: SerializeField]
    private float SlideForce { get; set; }

    [field: SerializeField, Header("Drag Amounts")]
    private float CleanDragAmount { get; set; }

    [field: SerializeField]
    private float NeutralDragAmount { get; set; }

    [field: SerializeField]
    private float DirtyDragAmount { get; set; }

    [field: SerializeField, Header("UI")]
    private Image CooldownBarImage { get; set; }

    [field: SerializeField]
    private GameObject ReadyFireOutline { get; set; }

    [field: SerializeField, Header("Particles")]
    private Transform ShootTransform { get; set; }

    [field: SerializeField]
    private List<GameObject> BubbleParticles { get; set; }

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
    private float m_BeamTimer;
    private bool m_IsSliding;
    private const float NORMAL_MOVEMENT_DRAG = 0;
    private int m_BubbleListLength;

    protected override void HandleAwake()
    {
        m_CurrTileType = TileType.NEUTRAL;
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_BubbleListLength = BubbleParticles.Count;
    }

    private void Update()
    {
        GetPlayerInputs();
        UpdateAim();
        UpdateCooldownBarUI();
        DisplayBubbleParticles();

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
        else
        {
            m_BeamTimer = Mathf.Min(m_BeamTimer + Time.deltaTime * 10f, BeamTime);
        }
    }

    private void UpdateCooldownBarUI()
    {
        var fraction = Mathf.Max(0f, m_BeamTimer / BeamTime);
        CooldownBarImage.fillAmount = fraction;
        ReadyFireOutline.SetActive(false);
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

        if (m_BeamTimer <= 0.5f)
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
                m_IsSliding = true;
                GlobalEvents.Player.OnPlayerShoot?.Invoke();
                return;
            }
            else
            {
                var slideMult = CurrTileType == TileType.CLEAN ? 3f : 1f;
                m_Rigidbody.AddForce(CurrentSlideDirection * SlideForce * slideMult);
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
        CurrentSlideDirection = -(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
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
        m_Rigidbody.drag = NORMAL_MOVEMENT_DRAG;
        GlobalEvents.Player.OnPlayerStopSliding?.Invoke();
    }

    private void DisplayBubbleParticles()
    {
        if (m_IsSliding && CommonUtil.JudgeExe(80))
        {
            var offset = Random.insideUnitCircle * 0.2f;
            Instantiate(BubbleParticles[Random.Range(0, m_BubbleListLength)], (Vector2)ShootTransform.position + offset, Quaternion.identity);
        }
    }
}
