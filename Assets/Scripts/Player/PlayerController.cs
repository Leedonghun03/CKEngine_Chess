using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    private PlayerInputHandler input;
    public float moveSpeed = 5.0f;    

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        Vector2 move = input.moveVector;
        Vector3 moveDir = new Vector3(move.x, 0.0f, move.y);
        
        transform.Translate(moveDir * (moveSpeed * Time.deltaTime), Space.World);
        //transform.forward = moveDir;

        if(moveDir.magnitude > 0.0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir, transform.up), 10.0f * Time.deltaTime);
        }
    }

    private void HandleInteraction()
    {
        if (input.pickTriggerd)
        {
            //Debug.Log("Pick or Place Triggerd");
            // 들기 or 놓기 처리
        }
    }
}
