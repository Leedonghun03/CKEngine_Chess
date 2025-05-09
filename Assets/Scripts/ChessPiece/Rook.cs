using UnityEngine;
using System.Collections.Generic;

public class Rook : Pieces
{
    [Header("첫 이동 여부")]
    [SerializeField] private bool hasMoved = false;
    
    private readonly List<Vector2Int> offsets = new()
    {
        // 앞
        new Vector2Int(0, 1),
        // 뒤
        new Vector2Int(0, -1),
        // 왼
        new Vector2Int(-1, 0),
        // 오른
        new Vector2Int(1, 0),
    };
    
    public override List<Vector2Int> GetAvailableMoves(Board chessBoard)
    {
        return SlideMoves(chessBoard, offsets);
    }
    
    public override List<Vector2Int> GetAttackSquares(Board chessBoard)
    {
        return GetAvailableMoves(chessBoard);
    }
    
    public override bool TryMoveTo(Vector2Int targetGridPosition)
    {
        if (!base.TryMoveTo(targetGridPosition))
        {
            return false;
        }

        if (!hasMoved)
        {
            hasMoved = true;
        }
        
        return true;
    }
}
