using UnityEngine;
using System.Collections.Generic;

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

    [Header("보드상 체스 말 위치")]
    public Vector2Int boardPosition;
    
    [Header("체스 보드")]
    [SerializeField] protected Board chessBoard;
    
    [Header("체스 말 이동 가능 위치")]
    [SerializeField] private List<Vector2Int> legalMoves;
    
    [Header("체스 말 이동 가능한 위치 표시 매니저")]
    [SerializeField] private MoveIndicatorManager indicatorManager;

    
    public void Awake()
    {
        if (chessBoard == null)
        {
            chessBoard = GameObject.Find("Chessboard").GetComponent<Board>();
        }

        if (indicatorManager == null)
        {
            indicatorManager = GameObject.Find("MoveIndicatorManager").GetComponent<MoveIndicatorManager>();
        }
    }

    // 캐릭터 hold pos로 옮기는 부분
    public void LiftToParent(Transform parent)
    {
        transform.SetParent(parent, false);
        transform.localPosition = Vector3.zero;
        
        legalMoves = GetAvailableMoves();
        indicatorManager.ShowMoveIndicator(chessBoard, legalMoves);
    }

    // 캐릭터가 체스 말 내려 놓는 부분
    public bool TryPlaceOnBoard(Vector3 worldPosition)
    {
        // worldPosition에서 gridPosition으로 변경
        Vector2Int gridPosition = chessBoard.WorldToGridPosition(worldPosition);
        
        if (legalMoves != null && legalMoves.Contains(gridPosition))
        {
            PerformMove(gridPosition);
            indicatorManager.ClearMoveIndicator();
            legalMoves = null;
            return true;
        }
        
        return false;
    }

    protected virtual List<Vector2Int> GetAvailableMoves() { return null; }
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
                        // 죽이거나 or 전투 진입
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
                // 죽이거나 or 전투 진입
                moves.Add(dest);
            }
        }

        return moves;
    }

    protected virtual void PerformMove(Vector2Int targetGridPosition)
    {
        // 보드 논리 갱신
        Vector2Int oldPieces = boardPosition;
        Vector2Int newPieces = targetGridPosition;
        
        chessBoard.SetPiece(oldPieces, null);
        chessBoard.UpdateAttackMap(this, false);

        chessBoard.SetPiece(newPieces, this);
        chessBoard.UpdateAttackMap(this, true);
        
        // 그리드 좌표에서 월드 좌표로 스냅
        transform.SetParent(chessBoard.transform, false);
        transform.position = chessBoard.GridToWorldPosition(targetGridPosition);
    }
}
