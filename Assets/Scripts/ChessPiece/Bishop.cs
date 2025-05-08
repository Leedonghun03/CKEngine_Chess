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
        List<Vector2Int> moves = new List<Vector2Int>();

        foreach (var moveOffset in offsets)
        {
            for (int step = 1; step < 8; step++)
            {
                Vector2Int dest = boardPosition + moveOffset * step;
                
                if (!chessBoard.IsInside(dest))
                {
                    break;
                }
                
                Pieces target = chessBoard.GetPiece(dest);
                if (target == null)
                {
                    moves.Add(dest);
                }
                else
                {
                    if (target.team != team)
                    {
                        moves.Add(dest);
                    }
                    break;
                }
            }
        }

        return moves;
    }
}
