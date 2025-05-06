using System;
using UnityEngine;
using System.Collections.Generic;

public enum TeamColor
{
    White,
    Black
}

public class Pieces : MonoBehaviour, ILiftAble
{
    public TeamColor team;

    public Vector2Int boardPosition;
    private Board chessBoard;
    
    // 이동 표시 매니저
    private MoveIndicatorManager indicatorManager;
    
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

    public void LiftToParent(Transform parent)
    {
        transform.SetParent(parent, false);
        transform.localPosition = Vector3.zero;
        
        List<Vector2Int> legalMoves = GetAvailableMoves(chessBoard);
        indicatorManager.ShowMoveIndicator(chessBoard, legalMoves);
    }

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
    
    public virtual bool TryMoveTo(Vector2Int targetGridPosition)
    {
        // 보드 범위 검사
        if (!chessBoard.IsInside(targetGridPosition))
        {
            return false;
        }
        
        // GetAvailableMoves 함수로 이동 합법성 검사
        List<Vector2Int> legalMoves = GetAvailableMoves(chessBoard);
        if (!legalMoves.Contains(targetGridPosition))
        {
            return false;
        }

        // 있던 자리 null
        chessBoard.SetPiece(boardPosition, null);
        // 보드 논리 갱신
        chessBoard.SetPiece(targetGridPosition, this);

        boardPosition = targetGridPosition;
        
        // 그리드 좌표에서 월드 좌표로 스냅
        Vector3 snappedWorldPosition = chessBoard.GridToWorldPosition(targetGridPosition);
        transform.SetParent(null, false);
        transform.position = snappedWorldPosition;
        
        return true;
    }
}
