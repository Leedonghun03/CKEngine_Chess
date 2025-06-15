using System.Linq;
using EndoAshu.Chess.InGame;
using EndoAshu.Chess.InGame.Pieces;
using Runetide.Util;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("카메라")]
    [SerializeField] public Camera Camera_Team_1;
    [SerializeField] public Camera Camera_Team_2;

    [Header("체스 보드")]
    [SerializeField] private Board chessBoard;

    [Header("고스트 소환 부모")]
    [SerializeField] public Transform ghostParent;
    
    [Header("체스말 휴지통")]
    [SerializeField] public Transform TrashCan;

    [Header("고스트 프리팹")]
    [SerializeField] public GameObject GhostControlPrefab;
    [SerializeField] public GameObject GhostNonControlPrefab;

    [Header("체스말 프리팹")]
    [SerializeField] public GameObject PawnPrefab;
    [SerializeField] public GameObject KnightPrefab;
    [SerializeField] public GameObject BishopPrefab;
    [SerializeField] public GameObject RookPrefab;
    [SerializeField] public GameObject QueenPrefab;
    [SerializeField] public GameObject KingPrefab;

    [Header("체스말 머티리얼")]
    [SerializeField] public Material MatBlackTeam;
    [SerializeField] public Material MatWhiteTeam;

    private UUID beforeUUID = UUID.NULL;

    #region 디버깅용

    [Header("현재 방 ID")]
    [OnlyViewVariable] public string RoomUUID = UUID.NULL.ToString();
    
    [Header("현재 방 인원")]
    [OnlyViewVariable] public int RoomMemberCount = -1;

    [Header("현재 턴 (숫자)")]
    [OnlyViewVariable] public int CurrentTurnNo = -1;

    [Header("현재 턴 (문자))")]
    [OnlyViewVariable] public ChessGamePlayingData.Turn CurrentTurn = ChessGamePlayingData.Turn.WHITE;

    #endregion

    void Start()
    {
        var id = ChessClientManager.Client.Account.UniqueId;

        int idx = 1;
        
        foreach (var i in ChessClientManager.Client.CurrentRoom.Members)
        {
            PlayerMoveController pmc;
            if (i.Id == id)
            {
                pmc = Instantiate(GhostControlPrefab, ghostParent).GetComponent<PlayerMoveController>();
            }
            else
            {
                pmc = Instantiate(GhostNonControlPrefab, ghostParent).GetComponent<PlayerMoveController>();
            }
            pmc.InitWhois(i.Id);
            pmc.transform.position = new Vector3(++idx * chessBoard.cellWorldSize, 0, 4 * chessBoard.cellWorldSize);
        }
        /*
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
        }*/
    }

    void Update()
    {
        var room = ChessClientManager.UnsafeClient?.CurrentRoom;
        if (room != null)
        {
            if (!room.PlayingData.IsPlaying)
            {
                return;
            }

            //var isTeam1 = room.Members.FirstOrDefault(e => e.Id == ChessClientManager.Client.Account.UniqueId)?.Mode == EndoAshu.Chess.Room.PlayerMode.TEAM1;
            var isTeam2 = room.Members.FirstOrDefault(e => e.Id == ChessClientManager.Client.Account.UniqueId)?.Mode == EndoAshu.Chess.Room.PlayerMode.TEAM2;
            //TODO : 관전자 모드 구현 편의성을 위해.
            //TODO : 지금은 관전자 구현 X

            Camera_Team_1.gameObject.SetActive(!isTeam2);
            Camera_Team_2.gameObject.SetActive(isTeam2);

            //Room의 PlayingData가 변경되었는지 확인
            if (room.PlayingData.InstanceId != beforeUUID)
            {
                beforeUUID = room.PlayingData.InstanceId;

                #region 디버깅용 대입
                CurrentTurnNo = room.PlayingData.TurnNo;
                CurrentTurn = room.PlayingData.CurrentTurn;
                RoomUUID = room.RoomId.ToString();
                RoomMemberCount = room.GetMemberCount();
                #endregion

                chessBoard.enPassantVulnerableX = room.PlayingData.enPassantVulnerable.Item1;
                chessBoard.enPassantVulnerableY = room.PlayingData.enPassantVulnerable.Item2;
                chessBoard.hasEnPassantVulnerable = room.PlayingData.hasEnPassantVulnerable;

                //Is Dirty
                foreach (var piece in FindObjectsByType<Pieces>(FindObjectsSortMode.None))
                {
                    chessBoard.UpdateAttackCoverageAt(piece, false);
                    piece.transform.SetParent(TrashCan);
                    piece.gameObject.SetActive(false);
                }

                var ht = room.PlayingData.HeldTarget;
                Vector2Int htvec = new Vector2Int(ht.Item1, ht.Item2);

                //PlayingData의 데이터에 맞게 동기화 작업
                for (int x = 0; x < 8; ++x)
                {
                    for (int y = 0; y < 8; ++y)
                    {
                        Vector2Int pos = new Vector2Int(x, y);

                        var pawn = room.PlayingData.Board[x, y];
                        if (pawn == null) continue;
                        //쓰레기통에서 찾아서 배치
                        var piece = CreateFromTrasnCan(pawn.PawnColor, pawn.PawnType, pos.x, pos.y);

                        //필드 동기화
                        if (piece is Pawn gamePawn)
                        {
                            if (pawn is EndoAshu.Chess.InGame.Pieces.Pawn inPawn)
                            {
                                gamePawn.isPromoted = inPawn.IsPromoted;
                            }
                            gamePawn.hasMoved = pawn.HasMoved;
                        }

                        chessBoard.SetPiece(piece, pos);
                        //현재 누가 들고있는건 굳이 렌더링해줄 필요는 없음

                        if (htvec == pos)
                        {
                            piece.gameObject.SetActive(false);
                        }
                    }
                }
                Debug.Log($"Current Player Held Pos : {htvec}");
                chessBoard.RebuildAttackMaps();
                
                TeamColor movedTeam = room.PlayingData.CurrentTurn == ChessGamePlayingData.Turn.WHITE
                    ? TeamColor.Black
                    : TeamColor.White;

                chessBoard.EvaluateCheckmate(movedTeam);
            }
        }
    }

    private Pieces CreateFromTrasnCan(ChessPawn.Color color, ChessPawn.TypeId id, int x, int y)
    {
        var piece = __CreateFromTrasnCan(color, id, x, y);
        float placeX = x * chessBoard.cellWorldSize;
        float placeY = y * chessBoard.cellWorldSize;
        piece.team = color == ChessPawn.Color.BLACK ? TeamColor.Black : TeamColor.White;
        piece.boardPosition = new Vector2Int(x, y);
        piece.transform.localPosition = new Vector3(placeX, 0, placeY);
        piece.transform.localRotation = new Quaternion(0, color == ChessPawn.Color.WHITE ? -180 : 0, 0, piece.transform.localRotation.w);
        piece.transform.localScale = Vector3.one;
        piece.GetComponent<MeshRenderer>().material = color == ChessPawn.Color.BLACK ? MatBlackTeam : MatWhiteTeam;
        //OnEnable 호출
        piece.gameObject.SetActive(true);
        return piece;
    }
    private Pieces __CreateFromTrasnCan(ChessPawn.Color color, ChessPawn.TypeId id, int x, int y)
    {
        for (int i = 0; i < TrashCan.childCount; ++i)
        {
            var child = TrashCan.GetChild(i);

            Pieces component = id switch
            {
                ChessPawn.TypeId.PAWN => child.GetComponent<Pawn>(),
                ChessPawn.TypeId.KNIGHT => child.GetComponent<Knight>(),
                ChessPawn.TypeId.ROOK => child.GetComponent<Rook>(),
                ChessPawn.TypeId.BISHOP => child.GetComponent<Bishop>(),
                ChessPawn.TypeId.QUEEN => child.GetComponent<Queen>(),
                ChessPawn.TypeId.KING => child.GetComponent<King>(),
                _ => null
            };

            if (component != null)
            {
                child.SetParent(chessBoard.gameObject.transform);
                return component;
            }
        }

        var gameObject = Instantiate(id switch
        {
            ChessPawn.TypeId.PAWN => PawnPrefab,
            ChessPawn.TypeId.KNIGHT => KnightPrefab,
            ChessPawn.TypeId.ROOK => RookPrefab,
            ChessPawn.TypeId.BISHOP => BishopPrefab,
            ChessPawn.TypeId.QUEEN => QueenPrefab,
            ChessPawn.TypeId.KING => KingPrefab,
            _ => null
        }, chessBoard.gameObject.transform);
        //OnEnable 초기화를 위한 기본 unactive상태
        gameObject.SetActive(false);
        return gameObject.GetComponent<Pieces>();
    }
}
