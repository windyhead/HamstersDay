using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
public class PopulationCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text populationText;
    [SerializeField] Transform defaultPoint;
    [SerializeField] Transform increaseEndPoint;
    [SerializeField] Transform decreaseEndPoint;
    [SerializeField] Transform increase;
    [SerializeField] Transform decrease;
    [SerializeField] TMP_Text increaseText;
    [SerializeField] TMP_Text decreaseText;
    
    private void Awake()
    {
        GameController.OnGameStarted += SetPopulation;
        PopulationSystem.OnPopulationChanged += UpdatePopulation;
    }

    private void SetPopulation()
    {
        populationText.text = PopulationSystem.Population.ToString();
    }
    
    private void OnDestroy()
    {
        PopulationSystem.OnPopulationChanged -= UpdatePopulation;
        GameController.OnPopulationChanged -= SetPopulation;
    }
    

    private void UpdatePopulation(int change)
    {
        populationText.text = PopulationSystem.Population.ToString();
        
        if(change > 0)
            increaseText.text = "+" + change;
        else
            decreaseText.text = change.ToString();
        
        StartCoroutine(MoveNumbers(change));
    }

    private IEnumerator MoveNumbers(int change)
    {
        if (change > 0)
        {
            increase.DOMove(defaultPoint.position, 0);
            increaseText.DOFade(1, 0);
            yield return new WaitForEndOfFrame();
            increase.DOMove(increaseEndPoint.position, 1);
            increaseText.DOFade(0, 2);
        }
        else
        {
            decrease.DOMove(defaultPoint.position, 0);
            decreaseText.DOFade(1, 0);
            yield return new WaitForEndOfFrame();
            decrease.DOMove(decreaseEndPoint.position, 1);
            decreaseText.DOFade(0, 2);
        }
    }
}
