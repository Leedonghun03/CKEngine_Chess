using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Board board; 

    void Start()
    {
        // 씬에 배치된 모든 Pieces를 찾아서 보드에 세팅
        foreach (var piece in FindObjectsOfType<Pieces>())
        {
            // 월드 위치 → 그리드 좌표
            Vector2Int gridPos = board.WorldToGridPosition(piece.transform.position);
            // 보드에 등록하면 piece.boardPosition도 자동으로 세팅됩니다
            board.SetPiece(gridPos, piece);
        }
    }
}
