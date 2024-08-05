using TMPro;
using UnityEngine;

public class PopulationCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text populationText;


    private void Awake()
    {
        GameController.OnPopulationChanged += UpdatePopulation;
    }

    private void OnDestroy()
    {
        TurnSystem.OnTurnFinished -= UpdatePopulation;
    }

    private void UpdatePopulation(int population)
    {
        populationText.text = population.ToString();
    }
}
