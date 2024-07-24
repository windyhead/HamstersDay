using TMPro;
using UnityEngine;

public class TurnCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text turnText;


    private void Awake()
    {
        TurnSystem.OnTurnFinished += UpdateTurn;
        UpdateTurn(1);
    }

    private void OnDestroy()
    {
        TurnSystem.OnTurnFinished -= UpdateTurn;
    }

    private void UpdateTurn(int turn)
    {
        turnText.text = turn.ToString();
    }
}
