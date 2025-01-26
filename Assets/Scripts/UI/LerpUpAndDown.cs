using UnityEngine;

public class LerpUpAndDown : MonoBehaviour
{
    public float m_Speed = 2f; // Speed of the movement
    public float m_Height = 1f; // Maximum height from the starting position

    private Vector3 m_StartPosition;

    private void Awake()
    {
        m_StartPosition = transform.position;
    }

    private void Update()
    {
        float newY = m_StartPosition.y + Mathf.Sin(Time.time * m_Speed) * m_Height;
        transform.position = new Vector3(m_StartPosition.x, newY, m_StartPosition.z);
    }
}
