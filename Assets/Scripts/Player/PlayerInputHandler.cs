using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputHandler : MonoBehaviour
{
    InputAction moveAction;
    InputAction pickUpAction;

    [FormerlySerializedAs("MoveVector")]
    public Vector2 moveVector;

    public bool pickTriggerd;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;

        pickUpAction = InputSystem.actions.FindAction("PickUp");
        pickUpAction.performed += OnPickPerformed;
        pickUpAction.canceled += OnPickCanceled;

        moveAction.Enable();
        pickUpAction.Enable();
    }

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
        pickTriggerd = true;
        Debug.Log("Pick Input : " + context.phase);
    }

    public void OnPickCanceled(InputAction.CallbackContext context)
    {
        pickTriggerd = false;
        Debug.Log("Pick Input : " + context.phase);
    }

    private void OnDestroy()
    {
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;

        pickUpAction.started -= OnPickPerformed;
        pickUpAction.canceled -= OnPickCanceled;
    }
}