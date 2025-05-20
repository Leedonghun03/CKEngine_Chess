using System;
using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    [Header("월드에서 체스 말들 간격")]
    private readonly float cellWorldSize = 1.25f;

    // 체스판 크기, 배열
    private const int gridSize = 8;
    // 이 보드 gird 배열 나중에 1차원으로 변경 필요
    private readonly Pieces[,] grid = new Pieces [gridSize, gridSize];
    
    // 각 팀의 기물들이 공격 가능한 위치 (체크메이트 or 캐슬링 판별)
    public readonly List<Pieces>[,] whiteAttackMap = new List<Pieces>[gridSize, gridSize];
    public readonly List<Pieces>[,] blackAttackMap = new List<Pieces>[gridSize, gridSize];
    
    // 8방향 벡터를 한 번만 생성해 두는 static 필드
    private readonly Vector2Int[] allDirections = {
        new ( 1, 0), // 동
        new (-1, 0), // 서
        new ( 0, 1), // 북
        new ( 0,-1), // 남
        new ( 1, 1), // 북동
        new (-1, 1), // 북서
        new ( 1,-1), // 남동
        new (-1,-1), // 남서
    };

    private void Awake()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                whiteAttackMap[x, y] = new List<Pieces>();
                blackAttackMap[x, y] = new List<Pieces>();
            }
        }
    }

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
    public void SetPiece(Pieces pieces, Vector2Int gridPos)
    {
        if (!IsInside(gridPos))
        {
            return;
        }
        
        grid[gridPos.x, gridPos.y] = pieces;

        if (pieces)
        {
            pieces.boardPosition = gridPos; 
        }
    }
    
    // 이동한 기물이 oldPiecesPos에서 newPiecesPos로 이동할 때
    // 관련된 공격 범위 맵을 일괄 업데이트하는 함수
    public void UpdateAttackMaps(Pieces movePieces, Vector2Int oldPiecesPos, Vector2Int newPiecesPos)
    {
        movePieces.boardPosition = oldPiecesPos;
        UpdateAttackCoverageAt(movePieces, false);
        
        movePieces.boardPosition = newPiecesPos;
        UpdateAttackCoverageAt(movePieces, true);
        
        foreach (Vector2Int dir in allDirections)
        {
            RefreshAttackMap(oldPiecesPos, dir);
            RefreshAttackMap(newPiecesPos, dir);
        }
    }
    
    // pivotPos 위치에서 dir(allDirections) 방향으로 탐색 후
    // 첫 번째로 만난 기물의 공격 범위를 제거 후 다시 추가 갱신합니다.
    // 즉, 이동으로 인해 막혔거나 열렸던 공격 경로를 다시 재계산하는 함수
    private void RefreshAttackMap(Vector2Int pivotPos, Vector2Int dir)
    {
        Vector2Int scan = pivotPos + dir;
        while (IsInside(scan))
        {
            Pieces otherPiece = GetPiece(scan);

            if (otherPiece)
            {
                UpdateAttackCoverageAt(otherPiece, false);
                UpdateAttackCoverageAt(otherPiece, true);
                break;
            }

            scan += dir;
        }
    }
    
    // 전달된 기물이 공격할 수 있는 모든 칸에 대해
    // 해당 팀의 공격 맵에 추가(add=true)하거나 제거(add=false)하는 함수
    public void UpdateAttackCoverageAt(Pieces pieces, bool add)
    {
        List<Pieces>[,] attackMap = pieces.team == TeamColor.White ? whiteAttackMap : blackAttackMap;

        foreach (Vector2Int attackPos in pieces.GetAttackSquares())
        {
            if (!IsInside(attackPos))
            {
                continue;
            }
            
            if(add)
            {
                if (!attackMap[attackPos.x, attackPos.y].Contains(pieces))
                {
                    attackMap[attackPos.x, attackPos.y].Add(pieces);
                }
            }
            else
            {
                if (attackMap[attackPos.x, attackPos.y].Contains(pieces))
                {
                    attackMap[attackPos.x, attackPos.y].Remove(pieces);
                }
            }
        }
    }
}
