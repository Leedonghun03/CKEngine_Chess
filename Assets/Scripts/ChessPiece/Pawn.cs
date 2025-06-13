using UnityEngine;
using System.Collections.Generic;

public class Pawn : Pieces
{
    [Header("첫 이동 여부")]
    [SerializeField] internal bool hasMoved = false;

    [Header("프로모션 여부 *true일 경우 웬만해서는 timeout임")]
    [SerializeField] internal bool isPromoted = false;
    
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

    public override List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        
        // 기본 이동
        Vector2Int oneMove  = boardPosition + new Vector2Int(0, dir);
        if (!chessBoard.GetPiece(oneMove))
        {
            moves.Add(oneMove);
            
            // 첫 이동시 2칸 이동
            if (!hasMoved)
            {
                Vector2Int twoMove = boardPosition + new Vector2Int(0, dir * 2);
                if (!chessBoard.GetPiece(twoMove))
                {
                    moves.Add(twoMove);
                }
            }
        }

        // 대각선 적 확인
        foreach (Vector2Int diagOffset in diagOffsets)
        {
            Pieces target = chessBoard.GetPiece(boardPosition + diagOffset);
            
            if (target && target.team != this.team)
            {
                Vector2Int attack = boardPosition + diagOffset;
                moves.Add(attack);
            }
        }

        // 앙파상
        // hasEnPassantVulnerable가 true이면 앙파상 이동 경로 추가
        if (chessBoard.hasEnPassantVulnerable)
        {
            int vx = chessBoard.enPassantVulnerableX;
            int vy = chessBoard.enPassantVulnerableY;
            
            // 같은 행에 있는지 확인
            if (Mathf.Abs(vx - boardPosition.x) == 1 && vy == boardPosition.y)
            {
                Vector2Int enPassantMovePoint = new Vector2Int(vx, vy + dir);

                if (!chessBoard.GetPiece(enPassantMovePoint))
                {
                    moves.Add(enPassantMovePoint);
                }
            }
        }
        
        return moves;
    }

    public override List<Vector2Int> GetAttackSquares()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        
        foreach (Vector2Int diagOffset in diagOffsets)
        {
            Vector2Int attack = boardPosition + diagOffset;
            moves.Add(attack);
        }
        
        return moves;
    }
    
    protected override void PerformMove(Vector2Int dropGridPosition)
    {
        Vector2Int oldPos = boardPosition;
        
        // 이동 두칸을 하면 Board.cs에 있는 앙파상 취약 좌표 설정 / 초기화
        if (Mathf.Abs(dropGridPosition.y - oldPos.y) == 2)
        {
            chessBoard.enPassantVulnerableX = dropGridPosition.x;
            chessBoard.enPassantVulnerableY = dropGridPosition.y;
            chessBoard.hasEnPassantVulnerable = true;
        }
        
        base.PerformMove(dropGridPosition);

        bool isPromotionRank = (team == TeamColor.White && dropGridPosition.y == 7) ||  (team == TeamColor.Black && dropGridPosition.y == 0);
        if (isPromotionRank)
        {
            chessBoard.ShowPawnPromotionUI(this);
        }
        
        if (!hasMoved)
        {
            hasMoved = true;
        }
    }
}