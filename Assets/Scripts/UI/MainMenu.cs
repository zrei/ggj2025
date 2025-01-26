using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [field: SerializeField, Header("Buttons")]
    private Button PlayButton { get; set; }

    [field: SerializeField]
    private Button TutorialButton { get; set; }

    [field: SerializeField]
    private Button QuitButton { get; set; }

    private void Awake()
    {
        PlayButton.onClick.RemoveAllListeners();
        PlayButton.onClick.AddListener(OnPlayButtonClick);
        TutorialButton.onClick.RemoveAllListeners();
        TutorialButton.onClick.AddListener(OnTutorialButtonClick);
        QuitButton.onClick.RemoveAllListeners();
        QuitButton.onClick.AddListener(OnQuitButtonClick);
    }

    private void OnPlayButtonClick()
    {
        SceneManager.LoadScene("Gameplay");
    }

    private void OnTutorialButtonClick()
    {
        SceneManager.LoadScene("Tutorial");
    }

    private void OnQuitButtonClick()
    {
        Application.Quit();
    }

}
