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

    public virtual List<Vector2Int> GetAvailableMoves(Board board) { return null; }

    public virtual bool TryMoveTo(Vector2Int targetPos, Board board)
    {
        // 범위 검사
        if (!board.IsInside(targetPos))
        {
            return false;
        }
        
        // GetAvailableMoves 함수로 이동 합법성 검사
        List<Vector2Int> legal = GetAvailableMoves(board);
        if (!legal.Contains(targetPos))
        {
            return false;
        }

        // 보드 논리 갱신
        board.SetPiece(boardPosition, null);
        board.SetPiece(targetPos, this);

        // 월드 위치로 스냅
        Vector3 worldPos = board.GridToWorldPosition(targetPos);
        PlaceAt(worldPos);

        return true;
    }
    
    public void LiftToParent(Transform parent)
    {
        transform.SetParent(parent, false);
        transform.localPosition = Vector3.zero;
    }

    public void PlaceAt(Vector3 worldPosition)
    {
        transform.SetParent(null, false);
        transform.position = worldPosition;
    }
}
