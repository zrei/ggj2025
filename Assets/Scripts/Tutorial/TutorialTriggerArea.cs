using UnityEngine;

public class TutorialTriggerArea : MonoBehaviour
{
    private bool m_HasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!m_HasTriggered)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                m_HasTriggered = true;
                TutorialManager.Instance.GoToNextTutorialStep();
            }
        }
    }
}
