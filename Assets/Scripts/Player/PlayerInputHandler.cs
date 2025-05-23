using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;

    [Header("입력 들어온 방향")]
    public Vector2 moveVector;
    [Header("스페이스 바 눌렸는지 확인")]
    public bool pickTriggered;

    private void Awake()
    {
        playerInput =  GetComponent<PlayerInput>();
    }
    
    private void OnEnable()
    {
        playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnDisable()
    {
        playerInput.onActionTriggered -= OnActionTriggered;
    }

    void OnActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move")
        {
            if (context.performed)
            {
                moveVector = context.ReadValue<Vector2>();
            }
            else if(context.canceled)
            {
                moveVector = Vector2.zero;
            }
        }

        if (context.action.name == "PickUp" && context.started)
        {
            pickTriggered = true;
        }
    }

    private void LateUpdate()
    {
        pickTriggered = false;
    }
}