using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    private PlayerInputHandler input;
    
    [Header("이동 속도")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float turnSpeed = 15.0f;

    [Header("잡기, 놓기 설정")]
    [SerializeField] private Transform holdPoint;         // 플레이어 머리 위 빈 오브젝트
    [SerializeField] private float rayDistance = 0.5f;
    private ILiftAble heldLiftAble;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
        
        Debug.DrawRay(transform.position + Vector3.up, transform.forward, Color.red);
    }
    
    // 플레이어 wasd 입력으로 이동
    private void HandleMovement()
    {
        Vector2 move = input.moveVector;
        Vector3 moveDir = new Vector3(move.x, 0.0f, move.y);

        transform.Translate(moveDir * (moveSpeed * Time.deltaTime), Space.World);

        if (moveDir.magnitude > 0.0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir, transform.up), turnSpeed * Time.deltaTime);
        }
    }

    // 플레이어 space바 입력
    private void HandleInteraction()
    {
        // space가 눌렸을 때만
        if (!input.pickTriggerd)
        {
            return;
        }

        // 이미 들고 있다면 -> 놓기
        if (heldLiftAble != null)
        {
            PlaceHeldPiece();
        }
        else
        {
            GrabPiece();
        }
        
        input.pickTriggerd = false;
    }
    
    // 체스말 들기
    void GrabPiece()
    {
        // 들고 있는게 없으면 레이로 탐색 후 집기
        Vector3 origin = transform.position + Vector3.up;
        Vector3 forward = transform.forward;

        if (Physics.Raycast(origin, forward, out RaycastHit hit, rayDistance))
        {
            heldLiftAble = hit.collider.GetComponent<ILiftAble>();

            if (heldLiftAble == null)
            {
                return;
            }
            
            heldLiftAble.LiftToParent(holdPoint);
        }
    }
    
    // 체스말 내려놓기
    void PlaceHeldPiece()
    {
        // 월드에 다시 놓을 월드 위치 계산 (플레이어 앞 1유닛)
        Vector3 dropWorldPos = holdPoint.position + transform.forward * 1.0f;           
        
        if (heldLiftAble.TryPlaceOnBoard(dropWorldPos))
        {
            heldLiftAble = null;
        }
    }
}
