using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BouncyObstacle : MonoBehaviour
{
    [SerializeField] private PhysicsMaterial2D m_BouncyMaterial;

    private Collider2D m_Collider2D;

    private void Start()
    {
        m_Collider2D = GetComponent<Collider2D>();    
    }

    private void Awake()
    {
        GlobalEvents.Player.OnPlayerStartSliding += OnPlayerStartSliding;
        GlobalEvents.Player.OnPlayerStopSliding += OnPlayerStopSliding;
    }

    private void OnDestroy()
    {
        GlobalEvents.Player.OnPlayerStartSliding -= OnPlayerStartSliding;
        GlobalEvents.Player.OnPlayerStopSliding -= OnPlayerStopSliding;
    }

    private void OnPlayerStartSliding()
    {
        m_Collider2D.sharedMaterial = m_BouncyMaterial;
    }

    private void OnPlayerStopSliding()
    {
        m_Collider2D.sharedMaterial = null;
    }
}
