using UnityEngine;
using System.Collections.Generic;

public class Pawn : Pieces
{
    [Header("첫 이동 여부")]
    [SerializeField] private bool hasMoved = false;
    
    [Header("이동 방향")]
    [SerializeField] private int dir;    
    
    private List<Vector2Int> diagOffsets;
    
    private void OnEnable()
    {
        // 팀에 따라서 이동 방향 정하기
        if (team == TeamColor.White)
        {
            dir = 1;
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

    protected override List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        
        // 기본 이동
        Vector2Int oneMove  = boardPosition + new Vector2Int(0, dir);
        if (!chessBoard.GetPiece(oneMove))
        {
            moves.Add(oneMove);
        }

        // 첫 이동시 2칸 이동
        if (!hasMoved)
        {
            Vector2Int twoMove = boardPosition + new Vector2Int(0, dir * 2);
            if (!chessBoard.GetPiece(twoMove))
            {
                moves.Add(twoMove);
            }
        }

        // 대각선 적 확인
        foreach (Vector2Int diagOffset in diagOffsets)
        {
            Pieces target = chessBoard.GetPiece(diagOffset);
            
            if (target && target.team != this.team)
            {
                // 죽이거나 or 전투 진입
                moves.Add(diagOffset);
            }
        }

        return moves;
    }

    public override List<Vector2Int> GetAttackSquares()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        
        foreach (Vector2Int diagOffset in diagOffsets)
        {
            Pieces target = chessBoard.GetPiece(diagOffset);
            
            if (target && target.team != this.team)
            {
                // 죽이거나 or 전투 진입
                moves.Add(diagOffset);
            }
        }
        
        return moves;
    }
    
    protected override void PerformMove(Vector2Int targetGridPosition)
    {
        base.PerformMove(targetGridPosition);

        if (!hasMoved)
        {
            hasMoved = true;
        }
    }
}