using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public abstract class Pieces : MonoBehaviour, ILiftAble
{
    public TeamColor team;
    public Vector2Int boardPosition;

    public abstract List<Vector2Int> GetAvailableMoves(Board board);
    
    public bool TryMoveTo(Vector2Int targetPos, Board board)
    {
        // 1) 유효 범위 & 합법 위치 검사
        if (!board.IsInside(targetPos))
        {
            return false;
        }
        
        List<Vector2Int> legal = GetAvailableMoves(board);
        if (!legal.Contains(targetPos))
        {
            return false;
        }

        // 2) 보드 논리 갱신
        board.SetPiece(boardPosition, null);
        board.SetPiece(targetPos, this);
        boardPosition = targetPos;

        // 3) 월드 위치로 스냅
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
