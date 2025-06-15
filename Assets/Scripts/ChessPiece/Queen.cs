using UnityEngine;
using System.Collections.Generic;

public class Queen : Pieces
{
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
        
        // 앞 오른 대각
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        // 뒤 왼 대각
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
    };

    public override List<Vector2Int> GetAvailableMoves()
    {
        return SlideMoves(offsets);
    }
    
    public override List<Vector2Int> GetAttackSquares()
    {
        return GetAvailableMoves();
    }
}
