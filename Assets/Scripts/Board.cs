using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    // 월드 크기 조정 값
    private float cellWorldSize = 1.25f;
    
    private const int gridSize = 8;
    private Pieces[,] grid = new Pieces [gridSize, gridSize];
    
    private int[,] whiteAttackMap = new int[gridSize, gridSize];
    private int[,] blackAttackMap = new int[gridSize, gridSize];

    // 월드 공간 좌표를 그리드 인덱스로 변환
    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellWorldSize);
        int y = Mathf.RoundToInt(worldPos.z / cellWorldSize);
        return new Vector2Int(x, y);
    }
    
    // 그리드 인덱스를 월드 공간 좌표로 변환
    public Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellWorldSize, 0, gridPos.y * cellWorldSize);
    }
    
    // 주어진 그리드 위치가 보드 내부인지 확인
    public bool IsInside(Vector2Int point)
    {
        if (point.x >= 0 && point.x < 8 && point.y >= 0 && point.y < 8)
        {
            return true;
        }

        return false;
    }
    
    // 유효한 그리드 위치에 있는 Pieces 객체를 반환
    public Pieces GetPiece(Vector2Int gridPos)
    {
        // 보드 외부면 null 반환
        if (!IsInside(gridPos))
        {
            return null;
        }

        return grid[gridPos.x, gridPos.y];
    }

    // 유요한 그리드 위치에 Pieces 객체를 설정
    public void SetPiece(Vector2Int gridPos, Pieces pieces)
    {
        if (!IsInside(gridPos))
        {
            return;
        }
        
        grid[gridPos.x, gridPos.y] = pieces;
        
        if (pieces != null)
            pieces.boardPosition = gridPos; 
    }

    public void UpdateAttackMap(Pieces pieces, bool add)
    {
        int count = add ? 1 : -1;
        int[,] attackTeamMap = pieces.team == TeamColor.White ? whiteAttackMap : blackAttackMap;

        foreach (Vector2Int attackPos in pieces.GetAttackSquares(this))
        {
            if(!IsInside(attackPos))
            {
                continue;
            }

            attackTeamMap[attackPos.x, attackPos.y] += count;
        }
    }
}
