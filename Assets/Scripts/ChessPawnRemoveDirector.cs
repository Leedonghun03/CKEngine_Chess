using System;
using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.InGame;
using EndoAshu.Chess.InGame.Pieces;
using Runetide.Packet;
using UnityEngine;

class ChessPawnRemoveDirector : MonoBehaviour
{

    [Header("체스 보드")]
    [SerializeField] private Board chessBoard;

    [Header("체스말 제거 프리팹")]
    [SerializeField] public GameObject PawnPrefab;
    [SerializeField] public GameObject KnightPrefab;
    [SerializeField] public GameObject BishopPrefab;
    [SerializeField] public GameObject RookPrefab;
    [SerializeField] public GameObject QueenPrefab;
    [SerializeField] public GameObject KingPrefab;

    [Header("체스말 제거 머티리얼")]
    [SerializeField] public Material blackTeam;
    [SerializeField] public Material whiteTeam;

    [Header("체스말 제거 부모")]
    [SerializeField] public Transform efxTransform;

    void Start()
    {
        if (ChessClientManager.Client.State is GameInState gis)
        {
            gis.Client.AddOnReceivePacketListener(OnPawnDestroyPacket);
        }
    }

    void OnDestroy()
    {
        if (ChessClientManager.Client.State is GameInState gis)
        {
            gis.Client.RemoveOnReceivePacketListener(OnPawnDestroyPacket);
        }
    }


    private void OnPawnDestroyPacket(IPacket pk)
    {
        if (pk is ChessPawnRemovedPacket cprp)
        {
            ChessClientManager.Client.RunOnMainThread(() => {            
                foreach (var pos in cprp.RemovedPositions)
                {
                    GameObject prefab = GetPrefab(pos.Item4);
                    var obj = Instantiate(prefab, efxTransform);
                    var pawn = obj.GetComponent<FracturePawn>();
                    pawn.material = pos.Item3 == ChessPawn.Color.WHITE ? whiteTeam : blackTeam;
                    pawn.transform.position = new Vector3(pos.Item1 * chessBoard.cellWorldSize, 0, pos.Item2 * chessBoard.cellWorldSize);
                }
            });
        }
    }

    private GameObject GetPrefab(ChessPawn.TypeId id)
    {
        return id switch
        {
            ChessPawn.TypeId.PAWN => PawnPrefab,
            ChessPawn.TypeId.KNIGHT => KnightPrefab,
            ChessPawn.TypeId.ROOK => RookPrefab,
            ChessPawn.TypeId.BISHOP => BishopPrefab,
            ChessPawn.TypeId.QUEEN => QueenPrefab,
            ChessPawn.TypeId.KING => KingPrefab,
            _ => null
        };
    }
}