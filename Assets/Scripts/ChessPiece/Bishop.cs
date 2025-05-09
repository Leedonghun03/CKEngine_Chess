using UnityEngine;
using System.Collections.Generic;

public class Bishop : Pieces
{
    private readonly List<Vector2Int> offsets = new()
    {
        // 앞 오른 대각
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        // 뒤 왼 대각
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
    };
    
    public override List<Vector2Int> GetAvailableMoves(Board chessBoard)
    {
        return SlideMoves(chessBoard, offsets);
    }
    
    public override List<Vector2Int> GetAttackSquares(Board chessBoard)
    {
        return GetAvailableMoves(chessBoard);
    }
}
