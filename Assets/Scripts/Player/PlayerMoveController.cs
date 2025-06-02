using System;
using System.Linq;
using EndoAshu.Chess.InGame;
using Runetide.Packet;
using Runetide.Util;
using UnityEngine;

class PlayerMoveController : MonoBehaviour
{
    [Header("플레이어 디버깅")]
    [OnlyViewVariable][SerializeField] private string uniqueId = UUID.NULL.ToString();
    [Header("이 클라이언트의 유저와 같은 팀인지?")]
    [OnlyViewVariable][SerializeField] private bool isPlayerTeam = false;

    internal UUID UniqueId = UUID.NULL;

    public GameObject MaterialTarget;

    public Material ownTeamMaterial;
    public Material otherTeamMaterial;
    private Vector3 targetPosition = Vector3.zero;
    //마지막 키입력 관련 (앵글처리)
    private Vector3 lookPosition = Vector3.zero;

    public void InitWhois(UUID id)
    {
        UniqueId = id;
        uniqueId = id.ToString();
        var currentMode = ChessClientManager.UnsafeClient?.CurrentRoom.Members.FirstOrDefault(e => e.Id == id)?.Mode;
        var playerId = ChessClientManager.UnsafeClient?.Account.UniqueId;
        var playerMode = ChessClientManager.UnsafeClient?.CurrentRoom.Members.FirstOrDefault(e => e.Id == playerId)?.Mode;

        Material targetMat;
        if (playerMode != null && currentMode == playerMode)
        {
            targetMat = ownTeamMaterial;
            isPlayerTeam = true;
        }
        else
        {
            targetMat = otherTeamMaterial;
            isPlayerTeam = false;
        }

        MaterialTarget.GetComponent<SkinnedMeshRenderer>().material = targetMat;
    }

    public void Start()
    {
        ChessClientManager.UnsafeClient?.AddOnReceivePacketListener(HandlePacket);
        targetPosition = transform.position;
    }

    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5.0f);

        /*
        Vector3 moveDir = targetPosition - transform.position;
        if (moveDir.magnitude > 0.1f)
        {
            //TODO
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir, transform.up), 5.0f * Time.deltaTime);
        }*/
        if (lookPosition.sqrMagnitude > 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPosition, transform.up), 5.0f * Time.deltaTime);
    }

    public void OnDestroy()
    {
        ChessClientManager.UnsafeClient?.RemoveOnReceivePacketListener(HandlePacket);
    }

    private void HandlePacket(IPacket t)
    {
        if (t is ChessGhostMovePacket pk)
        {
            if (pk.Id == UniqueId)
            {
                Handle(pk);
            }
        }
    }

    public void Handle(ChessGhostMovePacket pk)
    {
        //float deltaTime = ((DateTime.UtcNow - DateTime.UnixEpoch).Ticks - pk.Timestamp) / TimeSpan.TicksPerSecond;
        Vector3 position = new Vector3(pk.Position.Item1, pk.Position.Item2, pk.Position.Item3);
        lookPosition = new Vector3(pk.Velocity.Item1, pk.Velocity.Item2, pk.Velocity.Item3);
        targetPosition = position;// + velocity * deltaTime;
    }
}