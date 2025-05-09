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
    public TeamColor team = TeamColor.NoTeam;

    public Vector2Int boardPosition;
    [SerializeField] private Board chessBoard;
    
    // 이동 표시 매니저
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
        
        List<Vector2Int> legalMoves = GetAvailableMoves(chessBoard);
        indicatorManager.ShowMoveIndicator(chessBoard, legalMoves);
    }

    // 캐릭터가 체스말 내려 놓는 부분
    public bool TryPlaceOnBoard(Vector3 worldPosition)
    {
        // worldPosition에서 gridPosition으로 변경
        Vector2Int gridPosition = chessBoard.WorldToGridPosition(worldPosition);
        
        if (TryMoveTo(gridPosition))
        {
            indicatorManager.ClearMoveIndicator();
            return true;
        }
        
        return false;
    }
    
    public virtual List<Vector2Int> GetAvailableMoves(Board chessBoard) { return null; }
    public virtual List<Vector2Int> GetAttackSquares(Board chessBoard) { return null; }

    // Bishop, Rook, Queen에 사용될 헬퍼 메서드
    protected List<Vector2Int> SlideMoves(Board chessBoard, List<Vector2Int> offSets)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        foreach (var moveOffset in offSets)
        {
            for (int step = 1; step < 8; step++)
            {
                Vector2Int dest = boardPosition + moveOffset * step;
                
                if (!this.chessBoard.IsInside(dest))
                {
                    break;
                }
                
                Pieces target = this.chessBoard.GetPiece(dest);
                if (target == null)
                {
                    moves.Add(dest);
                }
                else
                {
                    if (target != null && target.team != this.team)
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
    protected List<Vector2Int> LeaperMoves(Board chessBoard, List<Vector2Int> offsets, bool onlyCapture = false)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        foreach (var moveOffset in offsets)
        {
            Vector2Int dest = boardPosition + moveOffset;

            // 보드의 범위 확인
            if (!this.chessBoard.IsInside(dest))
            {
                continue;
            }
            
            Pieces target = this.chessBoard.GetPiece(dest);
            if (target == null && !onlyCapture)
            {
                moves.Add(dest);
            }
            else if (target != null && target.team != this.team)
            {
                // 죽이거나 or 전투 진입
                moves.Add(dest);
            }
        }

        return moves;
    }
    
    public virtual bool TryMoveTo(Vector2Int targetGridPosition)
    {
        // 보드 범위 검사
        if (!chessBoard.IsInside(targetGridPosition))
        {
            return false;
        }
        
        // GetAvailableMoves 함수로 이동 가능 검사
        List<Vector2Int> legalMoves = GetAvailableMoves(chessBoard);
        if (!legalMoves.Contains(targetGridPosition))
        {
            return false;
        }

        // 있던 자리 null
        chessBoard.SetPiece(boardPosition, null);
        chessBoard.UpdateAttackMap(this, false);
        
        // 보드 논리 갱신
        boardPosition = targetGridPosition;
        
        chessBoard.SetPiece(boardPosition, this);
        chessBoard.UpdateAttackMap(this, true);

        
        // 그리드 좌표에서 월드 좌표로 스냅
        Vector3 snappedWorldPosition = chessBoard.GridToWorldPosition(targetGridPosition);
        transform.SetParent(chessBoard.transform, false);
        transform.position = snappedWorldPosition;
        
        return true;
    }
}
