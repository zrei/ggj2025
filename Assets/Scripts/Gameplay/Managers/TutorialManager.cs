using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/* The outline of the tutorial:
 * Teach player move and different tile types (1)
 * Teach how to shoot (2)
 * Teach timer and quota (3)
 */

public class TutorialManager : Singleton<TutorialManager>
{
    [field: SerializeField, Header("Text Object")]
    private GameObject TutorialTextObjectParent { get; set; }

    [field: SerializeField]
    private TextMeshProUGUI TutorialText { get; set; }

    [field: SerializeField, Header("Move and Tile Types"), TextArea(2, 3)]
    private string WelcomeString { get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string MoveInstructionsString { get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string TileTypesText { get; set; }

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

    private async void GoToNextTutorialStep()
    {
        m_CurrentTutorialStep++;
        if (m_CurrentTutorialStep == 1)
        {

        }
        else if (m_CurrentTutorialStep == 2)
        {

        }
        else if (m_CurrentTutorialStep == 3)
        {

        }
        else
        {
            // End tutorial

            await UniTask.WaitForSeconds(3f);
            if (!this) return;

        }
    }
}
