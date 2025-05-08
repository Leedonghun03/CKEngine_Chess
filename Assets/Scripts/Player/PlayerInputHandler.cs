using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;

    //InputAction moveAction;
    //InputAction pickUpAction;

    [Header("입력 들어온 방향")]
    public Vector2 moveVector;
    [Header("스페이스 바 눌렸는지 확인")]
    public bool pickTriggered;

    private void Awake()
    {
        playerInput =  GetComponent<PlayerInput>();
        
        /*
        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;

        pickUpAction = InputSystem.actions.FindAction("PickUp");
        pickUpAction.performed += OnPickPerformed;
        pickUpAction.canceled += OnPickCanceled;

        moveAction.Enable();
        pickUpAction.Enable();
        */
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

    /*
    public void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }

    public void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveVector = Vector2.zero;
    }

    public void OnPickPerformed(InputAction.CallbackContext context)
    {
        pickTriggered = true;
    }

    public void OnPickCanceled(InputAction.CallbackContext context)
    {
        pickTriggered = false;
    }

    private void OnDestroy()
    {
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;

        pickUpAction.started -= OnPickPerformed;
        pickUpAction.canceled -= OnPickCanceled;
    }
    */
}