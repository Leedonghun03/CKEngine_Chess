using UnityEngine;
using System.Collections.Generic;

public class King : Pieces
{
    [Header("첫 이동 여부")]
    [SerializeField] private bool hasMoved = false;
    
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

    private readonly List<Vector2Int> castlingOffsets = new()
    {
        new Vector2Int(2, 0),
        new Vector2Int(-2, 0),
    };

    public override List<Vector2Int> GetAvailableMoves()
    {
        List<Vector2Int> moves = new();
        List<Pieces>[,] enemyAttackMap = team == TeamColor.White ? chessBoard.blackAttackMap : chessBoard.whiteAttackMap;
        
        foreach (var moveOffset in offsets)
        {
            Vector2Int dest = boardPosition + moveOffset;

            if (!chessBoard.IsInside(dest))
            {
                continue;
            }
            if (enemyAttackMap[dest.x, dest.y].Count > 0)
            {
                continue;
            }
            
            Pieces target = chessBoard.GetPiece(dest);
            if (!target)
            {
                moves.Add(dest);
            }
            else if (target && target.team != this.team)
            {
                moves.Add(dest);
            }
        }
        
        if (!hasMoved)
        {
            foreach (var castlingOffset in castlingOffsets)
            {
                if (CanCastling(castlingOffset, chessBoard))
                {
                    moves.Add(boardPosition + castlingOffset);
                }
            }
        }
        
        return moves;
    }
    
    public override List<Vector2Int> GetAttackSquares()
    {
        List<Vector2Int> squares = new();
        foreach (Vector2Int offset in offsets)
        {
            Vector2Int pos = boardPosition + offset;
            if (chessBoard.IsInside(pos))
            {
                squares.Add(pos);
            }
        }

        return squares;
    }
    
    protected override void PerformMove(Vector2Int dropGridPosition)
    {
        // (놓을 위치) - (원래 있던 위치) 해서 2칸 이동했는지 확인
        Vector2Int isTwoMove = dropGridPosition - boardPosition;
        
        // 캐슬링
        if (Mathf.Abs(isTwoMove.x) == 2)
        {
            ExecuteCastle(isTwoMove);
        }
        else
        {
            base.PerformMove(dropGridPosition);
        }

        if (!hasMoved)
        {
            hasMoved = true;
        }
    }

    // 캐슬링 가능 조건
    private bool CanCastling(Vector2Int offset, Board board)
    {
        // king 움직이는 사이드 기준으로 룩 가져오는 부분
        int side = offset.x > 0 ? 7 : 0;
        Vector2Int rookPos = new Vector2Int(side, boardPosition.y);
        
        Rook rook = board.GetPiece(rookPos) as Rook;

        if (!rook || rook.hasMoved)
        {
            return false;
        }
        
        // king과 rook 사이가 비어있는지 확인
        // Queen Side인 경우 3칸 확인 King이면 2칸 확인해야함
        int minX = Mathf.Min(this.boardPosition.x, rook.boardPosition.x);
        int maxX = Mathf.Max(this.boardPosition.x, rook.boardPosition.x);
        
        for (int x = minX + 1; x < maxX; x++)
        {
            if (board.GetPiece(new Vector2Int(x, boardPosition.y)))
            {
                return false;
            }
        }
        
        // 캔슬링하는 위치가 공격을 받고 있는지 확인
        List<Pieces>[,] enemyAttackMap = team == TeamColor.White ? board.blackAttackMap : board.whiteAttackMap;
        int dir = (int)Mathf.Sign(offset.x);
        int offSetX = Mathf.Abs(offset.x);
        
        for (int i = 1; i <= offSetX; i++)
        {
            int x = boardPosition.x + dir * i;
            
            if (enemyAttackMap[x, boardPosition.y].Count > 0)
            {
                return false;
            }
        }
        
        return true;
    }

    // 실제 캐슬링 이동하는 함수 코드
    private void ExecuteCastle(Vector2Int offset)
    {
        // === 킹 이동 ===
        Vector2Int newKingPos = this.boardPosition + offset;
        hasMoved = true;
        
        base.PerformMove(newKingPos);
        
        // === 룩 이동 ===
        int dir = (int)Mathf.Sign(offset.x);
        int rookPosX = dir > 0 ? 7 : 0;
        int rookPosY = team == TeamColor.White ? 0 : 7;
        
        Vector2Int oldRookPos = new Vector2Int(rookPosX, rookPosY);
        Vector2Int newRookPos = new Vector2Int(newKingPos.x - dir, rookPosY);
        Rook rook = chessBoard.GetPiece(oldRookPos) as Rook;
        
        chessBoard.SetPiece(null, oldRookPos);
        chessBoard.SetPiece(rook, newRookPos);
        
        chessBoard.UpdateAttackMaps(rook, oldRookPos, newRookPos);

        rook.transform.SetParent(chessBoard.transform, false);
        rook.transform.position = chessBoard.GridToWorldPosition(newRookPos);
        rook.hasMoved = true;
    }
}
