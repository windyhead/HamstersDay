using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static Action OnLeftPressed;
    public static Action onRightPressed;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
            OnLeftPressed.Invoke();
        
        if(Input.GetKeyDown(KeyCode.RightArrow))
            onRightPressed.Invoke();
    }
}
