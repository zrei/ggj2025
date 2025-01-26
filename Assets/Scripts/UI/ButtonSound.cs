using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Set this as child
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ButtonSound : MonoBehaviour
{
    private Button m_Button;
    private AudioSource m_Audiosource;

    private void Start()
    {
        m_Button = transform.parent.GetComponent<Button>();
        m_Audiosource = GetComponent<AudioSource>();
        m_Button.onClick.AddListener(PlaySound);
    }

    private void OnDestroy()
    {
        m_Button.onClick.RemoveListener(PlaySound);
    }

    private void PlaySound()
    {
        if (PlayerHudManager.IsReady)
            PlayerHudManager.Instance.Playsound(m_Audiosource.clip);
        else
            m_Audiosource.Play();
    }
}
