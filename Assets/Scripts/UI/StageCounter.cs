using TMPro;
using UnityEngine;

public class StageCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text stageText;
    
    private void Awake()
    {
        GameController.OnStageChanged += UpdateStage;
    }

    private void OnDestroy()
    {
        TurnSystem.OnTurnFinished -= UpdateStage;
    }

    private void UpdateStage(int turn)
    {
        stageText.text = turn.ToString();
    }
}