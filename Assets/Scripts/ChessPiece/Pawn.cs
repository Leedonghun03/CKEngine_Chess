using UnityEngine;
using System.Collections.Generic;

public class Pawn : Pieces
{
    [Header("첫 이동 여부")]
    [SerializeField] private bool hasMoved = false;
    
    [Header("이동 방향")]
    [SerializeField] private int dir;

    public void Start()
    {
        // 팀에 따라서 이동 방향 정하기
        if (team == TeamColor.White)
        {
            dir = +1;
        }
        else
        {
            dir = -1;
        }
    }

    public override List<Vector2Int> GetAvailableMoves(Board chessBoard)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        
        // 기본 이동
        Vector2Int oneMove  = boardPosition + new Vector2Int(0, dir);
        if (chessBoard.IsInside(oneMove) && chessBoard.GetPiece(oneMove) == null)
        {
            moves.Add(oneMove);
        }

        // 첫 이동시 2칸 이동
        if (!hasMoved)
        {
            Vector2Int twoMove = boardPosition + new Vector2Int(0, dir * 2);
            if (chessBoard.GetPiece(twoMove) == null)
            {
                moves.Add(twoMove);
            }
        }

        // 대각선 캡처
        foreach (int dx in new[]{ -1, 1 })
        {
            Vector2Int diag = boardPosition + new Vector2Int(dx, dir);
            Pieces target = chessBoard.GetPiece(diag);
            
            // 죽이거나 or 전투 진입
            if (target != null && target.team != this.team)
            {
                moves.Add(diag);
            }
        }

        return moves;
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