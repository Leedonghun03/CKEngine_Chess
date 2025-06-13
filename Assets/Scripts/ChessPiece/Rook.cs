using UnityEngine;
using System.Collections.Generic;

public class Rook : Pieces
{
    [Header("첫 이동 여부")]
    public bool hasMoved = false;

    
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

    public override List<Vector2Int> GetAvailableMoves()
    {
        return SlideMoves(offsets);
    }
    
    public override List<Vector2Int> GetAttackSquares()
    {
        return GetAvailableMoves();
    }
    
    protected override void PerformMove(Vector2Int dropGridPosition)
    {
        base.PerformMove(dropGridPosition);

        if (!hasMoved)
        {
            hasMoved = true;
        }
    }
}
