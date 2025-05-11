using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputHandler : MonoBehaviour
{
    InputAction MoveAction;
    InputAction PickUpAction;

    [FormerlySerializedAs("MoveVector")]
    public Vector2 moveVector;

    public bool pickTriggerd;

    private void Start()
    {
        MoveAction = InputSystem.actions.FindAction("Move");
        MoveAction.performed += OnMovePerformed;
        MoveAction.canceled += OnMoveCanceled;

        PickUpAction = InputSystem.actions.FindAction("PickUp");
        PickUpAction.started += OnPickStarted;
        PickUpAction.canceled += OnPickCanceled;

        MoveAction.Enable();
        PickUpAction.Enable();
    }

    public void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        Debug.Log("Move Input : " + moveVector);
    }

    public void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveVector = Vector2.zero;
        Debug.Log("Move Canceled - Stopping");
    }

    public void OnPickStarted(InputAction.CallbackContext context)
    {
        pickTriggerd = true;
    }

    public void OnPickCanceled(InputAction.CallbackContext context)
    {
        pickTriggerd = false;
    }

    private void OnDestroy()
    {
        MoveAction.performed -= OnMovePerformed;
        MoveAction.canceled -= OnMoveCanceled;

        PickUpAction.started -= OnPickStarted;
        PickUpAction.canceled -= OnPickCanceled;
    }
}