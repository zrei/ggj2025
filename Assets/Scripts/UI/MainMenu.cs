using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [field: SerializeField, Header("Buttons")]
    private Button PlayButton { get; set; }

    [field: SerializeField]
    private Button QuitButton { get; set; }

    private void Awake()
    {
        PlayButton.onClick.RemoveAllListeners();
        PlayButton.onClick.AddListener(OnPlayButtonClick);
        QuitButton.onClick.RemoveAllListeners();
        QuitButton.onClick.AddListener(OnQuitButtonClick);
    }

    private void OnPlayButtonClick()
    {
        SceneManager.LoadScene("Tutorial");
    }

    private void OnQuitButtonClick()
    {
        Application.Quit();
    }

}
