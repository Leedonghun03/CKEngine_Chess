using System.Collections.Generic;
using UnityEngine;

public class Knight : Pieces
{
    private readonly List<Vector2Int> offsets = new()
    {
        // 위
        new Vector2Int(1, 2),
        new Vector2Int(-1, 2),
        // 아래
        new Vector2Int(1, -2),
        new Vector2Int(-1, -2),
        // 왼쪽
        new Vector2Int(2, 1),
        new Vector2Int(2, -1),
        // 오른쪽
        new Vector2Int(-2, 1),
        new Vector2Int(-2, -1)
    };

    public override List<Vector2Int> GetAvailableMoves()
    {
        return LeaperMoves(offsets);
    }
    
    public override List<Vector2Int> GetAttackSquares()
    {
        return GetAvailableMoves();
    }
}
