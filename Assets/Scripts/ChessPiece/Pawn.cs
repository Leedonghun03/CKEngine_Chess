using UnityEngine;
using System.Collections.Generic;

public class Pawn : Pieces
{
    // 첫 이동 여부
    private bool hasMoved = false;
    
    public override List<Vector2Int> GetAvailableMoves(Board board)
    {
        var moves = new List<Vector2Int>();
        int dir;

        if (team == TeamColor.White)
        {
            dir = +1;
        }
        else
        {
            dir = -1;
        }
        
        Vector2Int one  = boardPosition + new Vector2Int(0, dir);
        if (board.IsInside(one) && board.GetPiece(one) == null)
        {
            moves.Add(one);
        }

        // 첫 이동 두 칸
        Vector2Int two = boardPosition + new Vector2Int(0, dir * 2);
        if (!hasMoved && board.IsInside(two) && board.GetPiece(one) == null && board.GetPiece(two) == null)
        {
            moves.Add(two);
            hasMoved = true;
        }

        // 대각선 캡처
        foreach (int dx in new[]{ -1, 1 })
        {
            Vector2Int diag = boardPosition + new Vector2Int(dx, dir);
            Pieces target = board.GetPiece(diag);
            if (board.IsInside(diag) && target != null && target.team != this.team)
            {
                moves.Add(diag);
            }
        }

        return moves;
    }

    public bool TryMoveTo(Vector2Int targetPos, Board board)
    {
        if (!base.TryMoveTo(targetPos, board))
        {
            return false;
        }
        
        hasMoved = true;
        return true;
    }
}