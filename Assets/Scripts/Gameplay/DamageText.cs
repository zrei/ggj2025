using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [field: SerializeField]
    private float TimeTillDisappear { get; set; }

    [field: SerializeField]
    private TextMeshPro DamageValueText { get; set; }

    private float m_Timer;

    private void Awake()
    {
        m_Timer = TimeTillDisappear;
    }

    public void Setup(int value)
    {
        DamageValueText.text = value.ToString();
    }

    private void Update()
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
