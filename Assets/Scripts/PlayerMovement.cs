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

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
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
        // TODO: Check what the player is moving on and adjust

        m_Rigidbody.velocity = m_MovementVector.normalized * NeutralMovementSpeed;
    }
}
