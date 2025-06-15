using System.Linq;
using EndoAshu.Chess.Client.InGame;
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

        GetComponent<PlayerMoveController>().InitWhois(ChessClientManager.UnsafeClient?.Account.UniqueId);

        moveCache = transform.position;
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

        moveTicks -= Time.deltaTime;

        if (moveTicks <= 0.0f && (moveCache - transform.position).sqrMagnitude > 0.01f)
        {
            if (ChessClientManager.UnsafeClient?.State is GameInState gis)
            {
                moveTicks = 0.05f;
                gis.SendGhostMove((moveCache.x, moveCache.y, moveCache.z), (latestMoveVector.x, 0, latestMoveVector.y));
            }
        }
    }

    //일정 주기마다 보내기 위함.
    Vector3 moveCache;
    Vector2 latestMoveVector = Vector2.zero;
    float moveTicks = 0.0f;

    // 플레이어 wasd 입력으로 이동
    private void HandleMovement()
    {
        bool reverse = ChessClientManager.UnsafeClient?.CurrentRoom?.Members
            .FirstOrDefault(e => e.Id == ChessClientManager.Client.Account.UniqueId)?.Mode == EndoAshu.Chess.Room.PlayerMode.TEAM2;
        Vector2 move = latestMoveVector = reverse ? -input.moveVector : input.moveVector;
        Vector3 moveDir = new Vector3(move.x, 0.0f, move.y);
        moveCache += moveDir * (moveSpeed * Time.deltaTime);
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
                Board board = GameObject.Find("Chessboard").GetComponent<Board>();

                if (board.playState == Board.BoardPlayState.CheckMate ||
                    board.playState == Board.BoardPlayState.Stalemate)
                    return;
                
                // 집을 수 있는 기물셋이 비어있다면 && 포함이 안되어 있다면 return
                if (board.playState == Board.BoardPlayState.Check && !board.canGrabPieceSet.Contains(pieces))
                    return;
                
                if (ChessClientManager.UnsafeClient?.State is GameInState gi)
                {
                    gi.PawnHeld(pieces.boardPosition.x, pieces.boardPosition.y).Then((e) =>
                    {
                        //Debug.Log($"Held {e.Commander}, {e.TargetX}, {e.TargetY}");
                        if (e.TargetX >= 0 && e.TargetX < 8 && e.TargetY >= 0 && e.TargetY < 8)
                        {
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
                var heldPosition = b.HeldTarget;
                int x = heldPosition.Item1;
                int y = heldPosition.Item2;

                gi.PawnPlace(pos.x, pos.y).Then((e) =>
                {
                    if (e.TargetX >= 0 && e.TargetX < 8 && e.TargetY >= 0 && e.TargetY < 8)
                    {
                        Board board = GameObject.Find("Chessboard").GetComponent<Board>();
                        var piece = board.GetPiece(new Vector2Int(e.TargetX, e.TargetY));
                        piece.boardPosition = new Vector2Int(x, y);
                        piece.TryPlaceOnBoard(dropWorldPos);
                    }
                }).Catch(e =>
                {
                    Debug.LogError(e);
                });
            }
        }
    }
}
