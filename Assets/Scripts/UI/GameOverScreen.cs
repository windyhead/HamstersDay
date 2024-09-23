using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text fatText;

    public void Show(int stage, int fat)
    {
        stageText.text = stage.ToString();
        fatText.text = fat.ToString();
    }
}
