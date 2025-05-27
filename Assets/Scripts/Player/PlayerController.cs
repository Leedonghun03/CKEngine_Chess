using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.InGame;
using Runetide.Util;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("플레이어 입력 핸들러")]
    [SerializeField] private PlayerInputHandler input;

    [Header("이동, 회전 속도")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float turnSpeed = 15.0f;

    [Header("잡기, 놓기 설정")]
    [SerializeField] private Transform holdPoint; // 플레이어 머리 위 빈 오브젝트
    [SerializeField] private float rayDistance = 0.5f;

    private bool IsHeld {
        get
        {
            var b = ChessClientManager.UnsafeClient?.CurrentRoom.PlayingData;
            if (b == null) return false;
            return b.HeldTarget.Item1 >= 0 && b.HeldTarget.Item1 < 8 && b.HeldTarget.Item2 >= 0 & b.HeldTarget.Item2 < 8;
        }
    }

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        // moveVector의 값이 zero가 아니면 움직임 로직 작동
        if (input.moveVector != Vector2.zero)
        {
            HandleMovement();
        }
        
        // PickTriggered가 true이면 체스말 들기 or 놓기 로직 작동
        if (input.pickTriggered)
        {
            HandleInteraction();
        }
        
        // 체스말 들기 or 놓기 사용하는 Ray 보기 위한 Debug 
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
        // 이미 들고 있다면 -> 놓기
        if (IsHeld)
        {
            PlaceHeldPiece();
        }
        else
        {
            GrabPiece();
        }
    }
    
    // 체스말 들기
    void GrabPiece()
    {
        // 들고 있는게 없으면 레이로 탐색 후 집기
        Vector3 origin = transform.position + Vector3.up;
        Vector3 forward = transform.forward;

        if (Physics.Raycast(origin, forward, out RaycastHit hit, rayDistance))
        {
            var heldLiftAble = hit.collider.GetComponent<ILiftAble>();

            if (heldLiftAble == null)
            {
                return;
            }

            if (heldLiftAble is Pieces pieces)
            {
                if (ChessClientManager.UnsafeClient?.State is GameInState gi)
                {
                    gi.PawnHeld(pieces.boardPosition.x, pieces.boardPosition.y).Then((e) =>
                    {
                        //Debug.Log($"Held {e.Commander}, {e.TargetX}, {e.TargetY}");
                        if (e.TargetX >= 0 && e.TargetX < 8 && e.TargetY >= 0 && e.TargetY < 8)
                        {
                            Board board = GameObject.Find("Chessboard").GetComponent<Board>();
                            var piece = board.GetPiece(new Vector2Int(e.TargetX, e.TargetY));
                            piece.LiftToParent(holdPoint);
                        }
                    }).Catch(e =>
                    {
                        Debug.LogError(e);
                    });
                }
            }
        }
    }
    
    // 체스말 내려놓기
    void PlaceHeldPiece()
    {
        // 월드에 다시 놓을 월드 위치 계산 (플레이어 앞 1유닛)
        Vector3 dropWorldPos = holdPoint.position + transform.forward * 1.0f;
        Board board = GameObject.Find("Chessboard").GetComponent<Board>();

        var b = ChessClientManager.UnsafeClient?.CurrentRoom.PlayingData;
        if (b == null) return;
        var piece = board.GetPiece(new Vector2Int(b.HeldTarget.Item1, b.HeldTarget.Item2));

        if (piece.IsCanPlaceOnBoard(dropWorldPos, out Vector2Int pos))
        {

            if (ChessClientManager.UnsafeClient?.State is GameInState gi)
            {
                gi.PawnPlace(pos.x, pos.y).Then((e) =>
                {
                   // Debug.Log($"Place {e.Commander}, {e.TargetX}, {e.TargetY}");
                    if (e.TargetX >= 0 && e.TargetX < 8 && e.TargetY >= 0 && e.TargetY < 8)
                    {
                        Board board = GameObject.Find("Chessboard").GetComponent<Board>();
                        var piece = board.GetPiece(new Vector2Int(e.TargetX, e.TargetY));
                        piece.TryPlaceOnBoard(dropWorldPos);

                        ChessClientManager.UnsafeClient?.CurrentRoom.PlayingData.MarkDirty();
                    }
                }).Catch(e => {
                    Debug.LogError(e);
                });
            }
        }
    }
}
