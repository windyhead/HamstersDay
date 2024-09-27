using TMPro;
using UnityEngine;

public class StaminaCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text turnText;
    
    private void Awake()
    {
        StaminaSystem.OnPlayerStaminaChanged += UpdateStamina;
        ActionSystem.OnPlayerStaminaChanged += UpdateStamina;
        UpdateStamina(20);
    }

    private void OnDestroy()
    {
        StaminaSystem.OnPlayerStaminaChanged -= UpdateStamina;
        ActionSystem.OnPlayerStaminaChanged -= UpdateStamina;
    }

    private void UpdateStamina(int stamina)
    {
        turnText.text = stamina.ToString();
    }
}
