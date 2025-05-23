using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Room;
using Runetide.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomUIBtnCallbacks : MonoBehaviour
{
    public void OnQuitBtn()
    {
        if (ChessClientManager.UnsafeClient?.State is GameRoomState rs)
        {
            rs.QuitRoom().Then(e =>
            {
                if (e == EndoAshu.Chess.Room.RoomQuitPacket.QuitStatus.KICKED
                || e == EndoAshu.Chess.Room.RoomQuitPacket.QuitStatus.SELF_QUIT)
                {
                    SceneManager.LoadScene("LobbyScene");
                }
            });
        }
    }

    public void SetPlayerModeTeam1() => SetPlayerModeBtn(PlayerMode.TEAM1);
    public void SetPlayerModeTeam2() => SetPlayerModeBtn(PlayerMode.TEAM2);
    public void SetPlayerModeTeamObserver() => SetPlayerModeBtn(PlayerMode.OBSERVER);

    public void SetPlayerModeBtn(PlayerMode mode)
    {
        if (ChessClientManager.UnsafeClient?.State is GameRoomState rs)
        {
            rs.SetPlayerMode(mode);
        }
    }

    public void OnStartBtn()
    {
        if (ChessClientManager.UnsafeClient?.State is GameRoomState rs)
        {
            rs.StartRoom().Then(e =>
            {
                Debug.Log(e);
            });
        }
    }
}
