using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHudManager : Singleton<PlayerHudManager>
{
    [field: SerializeField, Header("Health Bar")]
    private Slider PlayerHealthSlider { get; set; }
    [SerializeField] private Image m_PlayerIconIndicator;
    [SerializeField] private Sprite m_GreenPlayerIcon;
    [SerializeField] private Sprite m_OrangePlayerIcon;
    [SerializeField] private Sprite m_RedPlayerIcon;

    [field: SerializeField, Header("Stage Timer")]
    private GameObject StageTimerParent {  get; set; }

    [field: SerializeField]
    public TextMeshProUGUI StageTimerText { get; set; }

    [field: SerializeField, Header("Clean Threshold")]
    public TextMeshProUGUI CleanThresholdText { get; set; }
    [field: SerializeField] public TextMeshProUGUI CurrCleanedText { get; set; }
    [SerializeField] private Slider m_QuotaSlider;

    [field: SerializeField, Header("Wave Start Display")]
    private Animator WaveStartAnimator { get; set; }
    [SerializeField] private TextMeshProUGUI m_WaveText;

    [field: SerializeField, Header("Lose Game Display")]
    private GameObject LoseGameDisplayParent { get; set; }

    [field: SerializeField]
    private Button TryAgainButton { get; set; }

    [field: SerializeField]
    private Button MainMenuButton { get; set; }

    [field: SerializeField, Header("Sounds")]
    private AudioSource AudioSource { get; set; }

    [field: SerializeField]
    private AudioClip WhistleAudioClip { get; set; }

    [field: SerializeField]
    private AudioClip SadAudioClip { get; set; }

    private const string WAVE_CARD_FORMAT = "Wave {0}";

    protected override void HandleAwake()
    {
        Player.OnPlayerHealthValueChanged += OnPlayerHealthValueChanged;

        TryAgainButton.onClick.RemoveAllListeners();
        TryAgainButton.onClick.AddListener(OnTryAgainButtonClick);
        MainMenuButton.onClick.RemoveAllListeners();
        MainMenuButton.onClick.AddListener(OnMainMenuButtonClick);

        PlayerHealthSlider.maxValue = GlobalEvents.Player.PlayerMaxHealth;
        PlayerHealthSlider.value = GlobalEvents.Player.PlayerMaxHealth;
    }

    protected override void HandleDestroy()
    {
        Player.OnPlayerHealthValueChanged -= OnPlayerHealthValueChanged;
    }

    private void Update()
    {
        if (BattleManager.Instance.State == GameState.InGame)
        {
            StageTimerText.text = ConvertTimerToDisplay(BattleManager.Instance.StageTimer);
            int filledPercentage = Mathf.CeilToInt(GridManager.Instance.GetCleanedPercentage);
            CurrCleanedText.text = filledPercentage.ToString();
            m_QuotaSlider.value = filledPercentage;
        }
    }

    public void StartWave(bool incrementWave = true)
    {
        DisplayStartWaveUI(incrementWave).Forget();
    }

    private async UniTask DisplayStartWaveUI(bool incrementWave = true)
    {
        // TODO: Show "Get Ready" text
        if (incrementWave && !BattleManager.Instance.HasNextWave)
        {
            await UniTask.WaitForSeconds(3f);
            BattleManager.Instance.FinishGame();
            return;
        }

        // We get the time for the stage first to display
        
        int nextWaveNumber = incrementWave ? BattleManager.Instance.CurrentWave + 1 : BattleManager.Instance.CurrentWave;
        m_WaveText.text = string.Format(WAVE_CARD_FORMAT, nextWaveNumber);
        int timeForStage = DWave.GetDataById(nextWaveNumber).Value.WaveTime;
        m_QuotaSlider.maxValue = DWave.GetDataById(nextWaveNumber).Value.CleanThreshold;
        m_QuotaSlider.value = 0;
        CurrCleanedText.text = "0";
        CleanThresholdText.text = $"{DWave.GetDataById(nextWaveNumber).Value.CleanThreshold}";
        StageTimerText.text = ConvertTimerToDisplay(timeForStage);

        // Some buffer for the next wave display to animate
        WaveStartAnimator.Play("WaveStartMoveIn");
        await UniTask.WaitForSeconds(1.5f);
        if (!this) return;

        WaveStartAnimator.Play("WaveStartMoveOut");
        await UniTask.WaitForSeconds(1.5f);
        if (!this) return;

        BattleManager.Instance.NextWave(incrementWave);

        await UniTask.CompletedTask;
    }

    private string ConvertTimerToDisplay(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;

        string minutesDisplay = minutes < 10 ? $"0{minutes}" : $"{minutes}";
        string secondsDisplay = seconds < 10 ? $"0{seconds}" : $"{seconds}";

        return $"{minutesDisplay}:{secondsDisplay}";
    }

    public async UniTask DisplayEndWaveUI()
    {
        // TODO: Display the wave end UI

        AudioSource.PlayOneShot(WhistleAudioClip);
        bool wavePassed = BattleManager.Instance.CheckIfPlayerPassed();

        // Some buffer time between waves
        await UniTask.WaitForSeconds(1f);
        if (!this) return;

        if (wavePassed)
        {
            DisplayStartWaveUI().Forget();
        }
        else
        {
            AudioSource.PlayOneShot(SadAudioClip);
            LoseGameDisplayParent.SetActive(true);
        }

        await UniTask.CompletedTask;
    }

    private void OnPlayerHealthValueChanged(int value)
    {
        PlayerHealthSlider.value = value;
        float percentage = value / PlayerHealthSlider.maxValue;
        if (percentage > GlobalEvents.UI.GreenOrangeThreshold)
        {
            m_PlayerIconIndicator.sprite = m_GreenPlayerIcon;
        }
        else if (percentage > GlobalEvents.UI.OrangeRedThreshold)
        {
            m_PlayerIconIndicator.sprite = m_OrangePlayerIcon;
        }
        else
        {
            m_PlayerIconIndicator.sprite = m_RedPlayerIcon;
        }
    }

    private void OnTryAgainButtonClick()
    {
        LoseGameDisplayParent.SetActive(false);
        StartWave(false);
    }

    private void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Playsound(AudioClip audioclip)
    {
        AudioSource.PlayOneShot(audioclip);
    }
}
