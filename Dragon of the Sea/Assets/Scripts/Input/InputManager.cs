using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private InputActionAsset inputAsset;
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;
    public bool WasControlsEnabledBeforePause = true;


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

        playerActionMap = inputAsset.FindActionMap("Player");
        uiActionMap = inputAsset.FindActionMap("UI");
    }

    public void EnablePlayerControls()
    {
        uiActionMap.Disable();
        playerActionMap.Enable();
    }

    public void EnableUIControls()
    {
        playerActionMap.Disable();
        uiActionMap.Enable();
    }
}
