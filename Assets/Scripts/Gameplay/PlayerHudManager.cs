using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHudManager : Singleton<PlayerHudManager>
{
    [field: SerializeField]
    private Slider PlayerHealthBar { get; set; }

    [field: SerializeField, Header("Stage Timer")]
    private GameObject StageTimerParent {  get; set; }

    [field: SerializeField]
    private TextMeshProUGUI StageTimerText { get; set; }

    private void Setup()
    {
        
    }

    private void SetStageTimer()
    {

    }

    public void DisplayWaveComplete()
    {

    }
}
