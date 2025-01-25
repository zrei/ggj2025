using Cinemachine;
using Cysharp.Threading.Tasks;
using RedBlueGames.Tools.TextTyper;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/* The outline of the tutorial:
 * Teach player move (1)
 * Teach player different tile types (2)
 * Teach how to shoot (3)
 * Teach timer and quota (4)
 */

public class TutorialManager : Singleton<TutorialManager>
{
    public bool CanPlayerShoot { get; private set; }

    [field: SerializeField, Header("Text Object")]
    private GameObject TutorialTextObjectParent { get; set; }

    [field: SerializeField]
    private TextMeshProUGUI TutorialText { get; set; }

    [field: SerializeField]
    private TextTyper TutorialTextTyper { get; set; }

    [field: SerializeField, Header("Cameras")]
    private List<CinemachineVirtualCamera> RoomCameras { get; set; }

    [field: SerializeField, Header("Doors")]
    private List<GameObject> Doors { get; set; }

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

    [field: SerializeField, TextArea(2, 3)]
    private string TrashMonsterText { get; set; }

    [field: SerializeField, Header("Shoot Logic"), TextArea(2, 3)]
    private string HowToShootText { get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string SpeedReminderText { get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string PlayAroundText { get; set; }

    [field: SerializeField, Header("Timer and Quota"), TextArea(2, 3)]
    private string TeachTimerText { get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string TeachQuotaText {  get; set; }

    [field: SerializeField, TextArea(2, 3)]
    private string ReadyToGoText { get; set; }

    private int m_CurrentTutorialStep;
    private int m_CurrentCamera = 0;

    protected override void HandleAwake()
    {
        CanPlayerShoot = false;
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

            await UniTask.WaitForSeconds(1f);
            if (!this) return;

            Doors[0].SetActive(false);
        }
        else if (m_CurrentTutorialStep == 2)
        {
            TutorialTextObjectParent.SetActive(false);
            Doors[0].SetActive(true);
            SwitchCamera(1);

            await UniTask.WaitForSeconds(1f);
            if (!this) return;

            TutorialText.text = string.Empty;
            TutorialTextObjectParent.SetActive(true);
            PlaySentence(TileTypesText);

            await UniTask.WaitForSeconds(5f);
            if (!this) return;

            PlaySentence(NeutralTileText);

            await UniTask.WaitForSeconds(5f);
            if (!this) return;

            PlaySentence(DirtyTileText);

            await UniTask.WaitForSeconds(5f);
            if (!this) return;

            PlaySentence(CleanTileText);

            await UniTask.WaitForSeconds(5f);
            if (!this) return;

            PlaySentence(TrashMonsterText);

            await UniTask.WaitForSeconds(5f);
            if (!this) return;

            Doors[1].SetActive(false);
        }
        else if (m_CurrentTutorialStep == 3)
        {
            TutorialTextObjectParent.SetActive(false);
            Doors[1].SetActive(true);

            SwitchCamera(2);

            await UniTask.WaitForSeconds(1f);
            if (!this) return;

            TutorialText.text = string.Empty;
            TutorialTextObjectParent.SetActive(true);
            PlaySentence(HowToShootText);

            CanPlayerShoot = true;

            await UniTask.WaitForSeconds(5f);
            if (!this) return;

            PlaySentence(SpeedReminderText);

            await UniTask.WaitForSeconds(5f);
            if (!this) return;

            PlaySentence(PlayAroundText);

            await UniTask.WaitForSeconds(5f);
            if (!this) return;

            GoToNextTutorialStep();
        }
        else if (m_CurrentTutorialStep == 4)
        {
            var timer = 5f;
            PlaySentence(TeachTimerText);
            // Make timer flash
            var timerObject = PlayerHudManager.Instance.StageTimerText.gameObject;
            while (timer > 0)
            {
                timerObject.SetActive(!timerObject.activeSelf);

                await UniTask.WaitForSeconds(0.5f);
                if (!this) return;
                timer -= 0.5f;
            }
            timerObject.SetActive(true);

            timer = 5f;
            PlaySentence(TeachQuotaText);
            // Make timer flash
            var quotaObject = PlayerHudManager.Instance.CleanThresholdText.gameObject;
            while (timer > 0)
            {
                quotaObject.SetActive(!quotaObject.activeSelf);

                await UniTask.WaitForSeconds(0.5f);
                if (!this) return;
                timer -= 0.5f;
            }
            timerObject.SetActive(true);

            PlaySentence(ReadyToGoText);

            await UniTask.WaitForSeconds(3f);
            if (!this) return;

            // End tutorial and open door to next area
            Doors[2].SetActive(false);
        }
        else
        {
            SceneManager.LoadScene("Gameplay");
        }
    }

    private void PlaySentence(string sentence)
    {
        // Make sure to set sentence to string.Empty first!
        TutorialText.text = string.Empty;
        TutorialTextTyper.TypeText(sentence, 0.01f);
    }

    private void SwitchCamera(int i)
    {
        if (i == m_CurrentCamera)
        {
            return;
        }

        RoomCameras[i].Priority = 10;
        RoomCameras[m_CurrentCamera].Priority = 0;
    }
}
