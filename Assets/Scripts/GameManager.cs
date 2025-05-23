using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("체스 보드")]
    [SerializeField] private Board chessBoard; 

    void Start()
    {
        // 씬에 배치된 모든 Pieces를 찾아서 보드에 세팅하는 작업
        foreach (var piece in FindObjectsByType<Pieces>(FindObjectsSortMode.None))
        {
            // 월드 위치 -> 그리드 좌표
            Vector2Int gridPos = chessBoard.WorldToGridPosition(piece.transform.position);
            // 보드에 등록하면 piece.boardPosition도 자동으로 세팅
            chessBoard.SetPiece(piece, gridPos);
        }
        
        // 각 팀에 공격 가능한 위치 초기화
        // pawn, knight 외에는 pawn이 막고 있어서 공격 불가능
        foreach (var pawn in FindObjectsByType<Pawn>(FindObjectsSortMode.None))
        {
            chessBoard.UpdateAttackCoverageAt(pawn, true);
        }

        foreach (var knight in FindObjectsByType<Knight>(FindObjectsSortMode.None))
        {
            chessBoard.UpdateAttackCoverageAt(knight, true);
        }
    }
}
