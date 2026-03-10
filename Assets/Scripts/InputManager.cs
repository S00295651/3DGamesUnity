using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    public static ApplicationActions Actions { get; private set; } 

    void Awake()
    {
        Actions = new ApplicationActions();
        Actions.Enable();
        Actions.Game.Enable();
    }
}