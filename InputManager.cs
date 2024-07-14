using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public PlayerInputActions playerInputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    public void DisableInGameInput()
    {
        playerInputActions.Movement.Disable();
        playerInputActions.Combat.Disable();
        playerInputActions.Look.Disable();
    }

    public void EnableInGameInput()
    {
        playerInputActions.Movement.Enable();
        playerInputActions.Combat.Enable();
        playerInputActions.Look.Enable();
    }
}
