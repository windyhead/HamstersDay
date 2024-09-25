using TMPro;
using UnityEngine;

public class ScoreEntryPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text nameField;
    [SerializeField] private TMP_Text scoreField;
    [SerializeField] private TMP_Text fatField;
    [SerializeField] private TMP_Text stageField;

    public void Initialize(string name, int score, int fat, int stage)
    {
        nameField.text = name;
        scoreField.text = score.ToString();
        fatField.text = fat.ToString();
        stageField.text = stage.ToString();
    }
}
