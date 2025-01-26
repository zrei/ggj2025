using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class EndScene : MonoBehaviour
{
    [SerializeField] private Button m_ReturnToMainMenuButton;

    // Start is called before the first frame update
    private void Start()
    {
        m_ReturnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);    
    }

    private void OnDestroy()
    {
        m_ReturnToMainMenuButton.onClick.RemoveAllListeners();
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
