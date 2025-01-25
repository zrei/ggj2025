using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [field: SerializeField, Header("Buttons")]
    private Button PlayButton { get; set; }

    [field: SerializeField]
    private Button HelpButton { get; set; }

    [field: SerializeField]
    private Button CloseHelpPanelButton { get; set; }

    [field: SerializeField, Header("Panels")]
    private GameObject HelpPanel { get; set; }

    private void Awake()
    {
        PlayButton.onClick.RemoveAllListeners();
        PlayButton.onClick.AddListener(OnPlayButtonClick);
        HelpButton.onClick.RemoveAllListeners();
        HelpButton.onClick.AddListener(OnHelpButtonClick);
        CloseHelpPanelButton.onClick.RemoveAllListeners();
        CloseHelpPanelButton.onClick.AddListener(OnCloseHelpPanelButtonClick);
    }

    private void OnPlayButtonClick()
    {
        SceneManager.LoadScene("Tutorial");
    }

    private void OnHelpButtonClick()
    {
        HelpPanel.SetActive(true);
    }

    private void OnCloseHelpPanelButtonClick()
    {
        HelpPanel.SetActive(false);
    }
}
