using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text totalText;
    [SerializeField] private TMP_Text fatText;
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button addEntryButton;
    [SerializeField] private ScoreEntryPanel[] scorePanels;

    private void Awake()
    {
        addEntryButton.onClick.AddListener(AddEntry);
    }

    public void Show()
    {
        nameField.interactable = true;
        addEntryButton.interactable = true;
        stageText.text = GameController.CurrentStage.ToString();
        totalText.text = (GameController.CurrentStage + GameController.PlayersFat).ToString();
        fatText.text = GameController.PlayersFat.ToString();
        RefreshScores();
    }

    private void RefreshScores()
    {
        var scores = HighscoresManager.Instance.GetHighScores();
        foreach (var panel in scorePanels)
            panel.gameObject.SetActive(false);
        
        for (var i = 0; i < scores.Count; i++)
        {
            if(i >= scorePanels.Length)
                return;
            scorePanels[i].Initialize(scores[i].Name,scores[i].Score,scores[i].Fat,scores[i].Stage);
            scorePanels[i].gameObject.SetActive(true);
        }
    }

    private void AddEntry()
    {
        nameField.interactable = false;
        addEntryButton.interactable = false;
        HighscoresManager.Instance.AddScore(nameField.text);
        RefreshScores();
    }
}
