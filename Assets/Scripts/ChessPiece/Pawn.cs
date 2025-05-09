using UnityEngine;
using System.Collections.Generic;

public class Pawn : Pieces
{
    [Header("첫 이동 여부")]
    [SerializeField] private bool hasMoved = false;
    
    [Header("이동 방향")]
    [SerializeField] private int dir;

    private List<Vector2Int> diagOffsets;
    
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
        
        diagOffsets = new()
        {
            new Vector2Int(-1, dir),
            new Vector2Int(1, dir)
        };
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
        moves.AddRange(LeaperMoves(chessBoard, diagOffsets, true));

        return moves;
    }

    public override List<Vector2Int> GetAttackSquares(Board chessBoard)
    {
        return LeaperMoves(chessBoard, diagOffsets, false);
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