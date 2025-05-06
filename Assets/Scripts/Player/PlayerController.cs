using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    private PlayerInputHandler input;
    [Header("이동 속도")]
    public float moveSpeed = 5.0f;
    public float turnSpeed = 15.0f;

    [Header("잡기, 놓기 설정")]
    public Transform holdPoint;         // 플레이어 머리 위 빈 오브젝트
    public Board board;
    public float rayDistance = 0.5f;

    private ILiftAble heldLiftAble;
    private Pieces heldPieceLogic;

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
        // 들고 있는게 없으면 -> 레이로 탐색 후 집기
        var origin = transform.position + Vector3.up;
        var forward = transform.forward;

        if (Physics.Raycast(origin, forward, out RaycastHit hit, rayDistance))
        {
            ILiftAble liftAble = hit.collider.GetComponent<ILiftAble>();
            Pieces piece = hit.collider.GetComponent<Pieces>();
            
            if(liftAble == null || piece == null)
                return;
            
            Vector2Int oldGrid = board.WorldToGridPosition(piece.transform.position);
            board.SetPiece(oldGrid, null);
            
            liftAble.LiftToParent(holdPoint);
            
            heldLiftAble = liftAble;
            heldPieceLogic = piece;
        }
    }

    
    // 체스말 내려놓기
    void PlaceHeldPiece()
    {
        // 월드에 다시 놓을 월드 위치 계산 (플레이어 앞 1유닛)
        Vector3 dropWorldPos = holdPoint.position + transform.forward * 1.0f;            
        Vector2Int gridPos = board.WorldToGridPosition(dropWorldPos);

        bool moved = heldPieceLogic.TryMoveTo(gridPos, board);
        if (!moved)
        {
            Debug.LogWarning($"{heldPieceLogic.name}은(는) {gridPos}로 이동할 수 없습니다.");
            return;
        }

        heldLiftAble = null;
        heldPieceLogic = null;
    }
}
