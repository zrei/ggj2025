using System.Collections;
using System.Collections.Generic;
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
            }
            m_CurrTileType = value;
        }
    }

    private void Awake()
    {
        CurrTileType = TileType.NEUTRAL;
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        CurrTileType = GridManager.Instance.GetTileTypeAtWorldCoordinates(transform.position);
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
        m_Rigidbody.velocity = m_MovementVector.normalized * GetMovementSpeed();
    }

    private float GetMovementSpeed()
    {
        return CurrTileType switch
        {
            TileType.PAINTED => CleanMovementSpeed,
            TileType.DIRTY => DirtyMovementSpeed,
            TileType.NEUTRAL => NeutralMovementSpeed,
            _ => 0,
        };
    }
}
