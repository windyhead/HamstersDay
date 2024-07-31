using UnityEngine;

public class SingletonBehaviour : MonoBehaviour
{
    public static SingletonBehaviour  Instance { get; private set; }

    private void Awake()
    {
       
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}