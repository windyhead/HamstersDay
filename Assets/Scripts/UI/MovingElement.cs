using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MovingElement : MonoBehaviour
{
    public float timer;
    public float brake;
    [SerializeField] private Transform element;
    [SerializeField] private Transform startingTransform;
    [SerializeField] private Transform targetTr;

    public void OnEnable()
    {
        StartCoroutine(Move());
    }

    public void OnDisable()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        var time = Time.deltaTime;

        while (true)
        {
            element.transform.position = startingTransform.position;
            element.DOMove(targetTr.position, timer).SetEase(Ease.Linear);
            yield return new WaitForSeconds(timer * time + brake);
        }
    }
}
