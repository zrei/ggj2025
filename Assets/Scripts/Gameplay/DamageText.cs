using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [field: SerializeField]
    private float TimeTillDisappear { get; set; }

    [field: SerializeField]
    private TextMeshPro DamageValueText { get; set; }

    private float m_Timer;
    private bool m_IsSetUp = false;

    private void Awake()
    {
        m_Timer = TimeTillDisappear;
    }

    public void Setup(int value, Vector2 pos)
    {
        transform.position = pos;
        DamageValueText.text = value.ToString();
        m_IsSetUp = true;
    }

    private void Update()
    {
        if (m_IsSetUp)
        {
            if (m_Timer > 0)
            {
                m_Timer -= Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
