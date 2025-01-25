using Cysharp.Threading.Tasks;
using RedBlueGames.Tools.TextTyper;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/* The outline of the tutorial:
 * Teach player move (1)
 * Teach player different tile types (2)
 * Teach how to shoot (3)
 * Teach timer and quota (4)
 */

public class TutorialManager : Singleton<TutorialManager>
{
    [field: SerializeField, Header("Text Object")]
    private GameObject TutorialTextObjectParent { get; set; }

    [field: SerializeField]
    private TextMeshProUGUI TutorialText { get; set; }

    [field: SerializeField]
    private TextTyper TutorialTextTyper { get; set; }

    [field: SerializeField, Header("Move"), TextArea(2, 3)]
    private string WelcomeString { get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string MoveInstructionsString { get; set; }

    [field: SerializeField, Header("Tile Types"), TextArea(2, 3)]
    private string TileTypesText { get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string NeutralTileText { get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string DirtyTileText { get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string CleanTileText { get; set; }

    [field: SerializeField, Header("Shoot Logic"), TextArea(2, 3)]
    private string HowToShootText { get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string HowToSlideText { get; set; }

    [field: SerializeField, Header("Timer and Quota"), TextArea(2, 3)]
    private string TeachTimerText { get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string TeachQuotaText {  get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string ReadyToGoText { get; set; }

    private int m_CurrentTutorialStep;

    protected override void HandleAwake()
    {
        m_CurrentTutorialStep = 0;
        DelayBeforeStartingTutorial();
    }

    private async void DelayBeforeStartingTutorial()
    {
        await UniTask.WaitForSeconds(2f);
        if (!this) return;

        GoToNextTutorialStep();
    }

    public async void GoToNextTutorialStep()
    {
        m_CurrentTutorialStep++;
        if (m_CurrentTutorialStep == 1)
        {
            TutorialText.text = string.Empty;
            TutorialTextObjectParent.SetActive(true);
            PlaySentence(WelcomeString);
            await UniTask.WaitForSeconds(3f);
            if (!this) return;

            TutorialText.text = string.Empty;
            PlaySentence(MoveInstructionsString);
        }
        else if (m_CurrentTutorialStep == 2)
        {

        }
        else if (m_CurrentTutorialStep == 3)
        {

        }
        else if (m_CurrentTutorialStep == 4)
        {

        }
        else
        {
            TutorialTextObjectParent.SetActive(false);

            // End tutorial and open door to next area
            // TODO
        }
    }

    private void PlaySentence(string sentence)
    {
        // Make sure to set sentence to string.Empty first!
        TutorialText.text = string.Empty;
        TutorialTextTyper.TypeText(sentence, 0.01f);
    }
}
