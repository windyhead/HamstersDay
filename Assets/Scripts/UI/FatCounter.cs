using TMPro;
using UnityEngine;

public class FatCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text fatText;
    
    private void Awake()
    {
        FatSystem.OnPlayerFatIncreased += UpdatePlayerFat;
        
        UpdatePlayerFat(1);
    }

    private void OnDestroy()
    {
        FatSystem.OnPlayerFatIncreased -= UpdatePlayerFat;
    }

    private void UpdatePlayerFat(int fat)
    {
        fatText.text = fat.ToString();
    }
}
