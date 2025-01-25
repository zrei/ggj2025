using UnityEngine;

public class TutorialTriggerArea : MonoBehaviour
{
    private bool m_HasTriggered = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!m_HasTriggered)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                m_HasTriggered = true;
                TutorialManager.Instance.GoToNextTutorialStep();
            }
        }
    }
}
