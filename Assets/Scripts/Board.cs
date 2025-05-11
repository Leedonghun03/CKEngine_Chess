using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("월드에서 체스 말들 간격")]
    private readonly float cellWorldSize = 1.25f;

    // 체스판 크기, 배열
    private const int gridSize = 8;
    private readonly Pieces[,] grid = new Pieces [gridSize, gridSize];
    
    // 각 팀의 기물들이 공격 가능한 위치 (체크메이트 or 캐슬링 판별)
    public readonly int[,] whiteAttackMap = new int[gridSize, gridSize];
    public readonly int[,] blackAttackMap = new int[gridSize, gridSize];
    
    // 월드 공간 좌표를 그리드 인덱스로 변환하는 함수
    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellWorldSize);
        int y = Mathf.RoundToInt(worldPos.z / cellWorldSize);
        return new Vector2Int(x, y);
    }
    
    // 그리드 인덱스를 월드 공간 좌표로 변환하는 함수
    public Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellWorldSize, 0, gridPos.y * cellWorldSize);
    }
    
    // 주어진 그리드 위치가 보드 내부인지 확인하는 함수
    public bool IsInside(Vector2Int point)
    {
        if (point.x >= 0 && point.x < 8 && point.y >= 0 && point.y < 8)
        {
            return true;
        }

        return false;
    }
    
    // 유효한 그리드 위치에 있는 Pieces 객체를 반환하는 함수
    public Pieces GetPiece(Vector2Int gridPos)
    {
        // 보드 외부면 null 반환
        if (!IsInside(gridPos))
        {
            return null;
        }

        return grid[gridPos.x, gridPos.y];
    }

    // 유요한 그리드 위치에 Pieces 객체를 설정하는 함수
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

    // 적의 기물 공격 위치를 담아두기 위한 함수
    public void UpdateAttackMap(Pieces pieces, bool add)
    {
        int count = add ? 1 : -1;
        int[,] attackTeamMap = pieces.team == TeamColor.White ? whiteAttackMap : blackAttackMap;

        foreach (Vector2Int attackPos in pieces.GetAttackSquares())
        {
            if(IsInside(attackPos))
            {
                attackTeamMap[attackPos.x, attackPos.y] += count;
            }
        }
    }
}
