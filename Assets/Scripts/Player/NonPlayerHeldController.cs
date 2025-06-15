using System.Linq;
using EndoAshu.Chess.InGame.Pieces;
using Runetide.Util;
using UnityEngine;

class NonPlayerHeldController : MonoBehaviour
{

    [Header("체스말 프리팹")]
    [SerializeField] public GameObject PawnPrefab;
    [SerializeField] public GameObject KnightPrefab;
    [SerializeField] public GameObject BishopPrefab;
    [SerializeField] public GameObject RookPrefab;
    [SerializeField] public GameObject QueenPrefab;
    [SerializeField] public GameObject KingPrefab;

    [Header("체스말 머티리얼")]
    [SerializeField] public Material Material_Team_1;
    [SerializeField] public Material Material_Team_2;

    private UUID beforeUUID = UUID.NULL;

    [Header("체스말 들 transform")]
    [SerializeField] public Transform heldSlot;

    public void Update()
    {
        var room = ChessClientManager.UnsafeClient?.CurrentRoom;
        if (room != null)
        {
            if (room.Members.Count(e => e.Id == GetComponent<PlayerMoveController>().UniqueId) <= 0)
            {
                Destroy(gameObject);
                return;
            }

            if (!room.PlayingData.IsPlaying)
            {
                return;
            }

            if (room.PlayingData.InstanceId != beforeUUID)
            {
                beforeUUID = room.PlayingData.InstanceId;

#nullable enable
                GameObject? heldWhat = null;
                ChessPawn.Color c = ChessPawn.Color.BLACK;
#nullable disable
                Debug.Log($"{room.PlayingData.HeldWho} == {GetComponent<PlayerMoveController>().UniqueId}");
                if (room.PlayingData.HeldWho == GetComponent<PlayerMoveController>().UniqueId)
                {
                    var piece = room.PlayingData.Board[room.PlayingData.HeldTarget.Item1, room.PlayingData.HeldTarget.Item2];
                    c = piece.PawnColor;

                    heldWhat = piece.PawnType switch
                    {
                        ChessPawn.TypeId.BISHOP => BishopPrefab,
                        ChessPawn.TypeId.PAWN => PawnPrefab,
                        ChessPawn.TypeId.KNIGHT => KnightPrefab,
                        ChessPawn.TypeId.ROOK => RookPrefab,
                        ChessPawn.TypeId.QUEEN => QueenPrefab,
                        ChessPawn.TypeId.KING => KingPrefab,
                        _ => null
                    };
                }

                for (int i = heldSlot.childCount - 1; i >= 0; --i)
                    Destroy(heldSlot.GetChild(i).gameObject);

                if (heldWhat != null) {
                    var heldObj = Instantiate(heldWhat, heldSlot);
                    heldObj.transform.localPosition = Vector3.zero;
                    heldObj.transform.localScale = Vector3.one;
                    heldObj.transform.rotation = new Quaternion(0, c == ChessPawn.Color.BLACK ? 0 : -180, 0, 1);
                    heldObj.GetComponent<MeshRenderer>().material = c == ChessPawn.Color.BLACK ? Material_Team_2 : Material_Team_1;
                }
            }
        }
    }
}