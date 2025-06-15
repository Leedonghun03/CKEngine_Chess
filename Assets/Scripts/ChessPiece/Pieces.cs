using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Playables;

public enum TeamColor
{
    NoTeam = 0,
    White,
    Black
}

public class Pieces : MonoBehaviour, ILiftAble
{
    [Header("체스 말 팀")]
    public TeamColor team = TeamColor.NoTeam;

    [Header("체스 보드")]
    [SerializeField] protected Board chessBoard;
    
    [Header("보드상 체스 말 위치")]
    public Vector2Int boardPosition;
    public Vector3 worldPosition;
    
    [Header("체스 말 이동 가능 위치")]
    [SerializeField] private List<Vector2Int> legalMoves;
    
    [Header("체스 말 이동 가능한 위치 표시 매니저")]
    [SerializeField] private MoveIndicatorManager indicatorManager;

    
    public void Awake()
    {
        if (!chessBoard)
        {
            chessBoard = GameObject.Find("Chessboard").GetComponent<Board>();
        }

        if (!indicatorManager)
        {
            indicatorManager = GameObject.Find("MoveIndicatorManager").GetComponent<MoveIndicatorManager>();
        }
    }

    // 캐릭터 hold pos로 옮기는 부분
    public void LiftToParent(Transform parent)
    {
        worldPosition = transform.position;
        //transform.position = Vector3.zero;
        //transform.SetParent(parent, false);
        GetComponent<MeshRenderer>().enabled = false;
        
        legalMoves = chessBoard.TryGetCachedMoves(this);
        indicatorManager.ShowMoveIndicator(chessBoard, legalMoves);
    }

    public bool IsCanPlaceOnBoard(Vector3 dropWorldPosition, out Vector2Int boardPos)
    {
        boardPos = chessBoard.WorldToGridPosition(dropWorldPosition);
        if (boardPos == boardPosition)
            return true;
        if (legalMoves == null || !legalMoves.Contains(boardPos))
        {
            return false;
        }
        return true;
    }

    // 캐릭터가 체스 말 내려 놓는 부분
    public bool TryPlaceOnBoard(Vector3 dropWorldPosition)
    {
        // worldPosition에서 gridPosition으로 변경
        Vector2Int dropGridPosition = chessBoard.WorldToGridPosition(dropWorldPosition);
        GetComponent<MeshRenderer>().enabled = true;

        // 자기 자신 자리에 내려놓을 때
        // 서버에서는 턴을 안넘기는 처리 필요
        if (dropGridPosition == boardPosition)
        {
            transform.SetParent(chessBoard.transform, false);
            transform.position = worldPosition;
            indicatorManager.ClearMoveIndicator();
            legalMoves = null;
            return true;
        }
        
        chessBoard.hasEnPassantVulnerable = false;

        PerformMove(dropGridPosition);
        indicatorManager.ClearMoveIndicator();
        legalMoves = null;
        return true;
    }

    public virtual List<Vector2Int> GetAvailableMoves() { return null; }
    //pawn문 대각선 적 확인 하나 때문에 사용중임
    public virtual List<Vector2Int> GetAttackSquares() { return null; }

    // Bishop, Rook, Queen에 사용될 헬퍼 메서드
    protected List<Vector2Int> SlideMoves(List<Vector2Int> offSets)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        foreach (var moveOffset in offSets)
        {
            for (int step = 1; step < 8; step++)
            {
                Vector2Int dest = boardPosition + moveOffset * step;
                
                if (!chessBoard.IsInside(dest))
                {
                    break;
                }
                
                Pieces target = chessBoard.GetPiece(dest);
                if (!target)
                {
                    moves.Add(dest);
                }
                else
                {
                    if (target && target.team != this.team)
                    {
                        moves.Add(dest);
                    }
                    break;
                }
            }
        }

        return moves;
    }

    // Knight, king에 사용될 헬퍼 메서드, onlyCapture은 빈 칸 무시할지 말지 선택
    protected List<Vector2Int> LeaperMoves(List<Vector2Int> offsets)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        foreach (var moveOffset in offsets)
        {
            Vector2Int dest = boardPosition + moveOffset;

            if (!chessBoard.IsInside(dest))
            {
                continue;
            }
            
            Pieces target = chessBoard.GetPiece(dest);
            if (!target)
            {
                moves.Add(dest);
            }
            else if (target && target.team != this.team)
            {
                moves.Add(dest);
            }
        }

        return moves;
    }

    protected virtual void PerformMove(Vector2Int dropGridPosition)
    {
        // 그리드 논리 이동
        Vector2Int oldPiecesPos = boardPosition;
        chessBoard.SetPiece(null, oldPiecesPos);
        chessBoard.SetPiece(this, dropGridPosition);

        // 공격 맵 갱신
        chessBoard.UpdateAttackMaps(this, oldPiecesPos, dropGridPosition);

        // 상대 King 체크메이트 확인
        chessBoard.EvaluateCheckmate(this.team);

        // 그리드 좌표에서 월드 좌표로 스냅
        transform.SetParent(chessBoard.transform, false);
        transform.position = chessBoard.GridToWorldPosition(dropGridPosition);
    }
}
