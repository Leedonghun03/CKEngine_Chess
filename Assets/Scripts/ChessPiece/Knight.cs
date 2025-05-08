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

    public override List<Vector2Int> GetAvailableMoves(Board chessBoard)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        foreach (var moveOffset in offsets)
        {
            Vector2Int dest = boardPosition + moveOffset;

            // 보드의 범위 확인
            if (!chessBoard.IsInside(dest))
            {
                continue;
            }
            
            Pieces target = chessBoard.GetPiece(dest);
            if (target == null)
            {
                moves.Add(dest);
            }
            else if (target.team != team)
            {
                // 죽이거나 or 전투 진입
                moves.Add(dest);
            }
        }

        return moves;
    }
}
