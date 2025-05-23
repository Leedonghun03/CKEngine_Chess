using System;
using EndoAshu.Chess.Client.State;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoChange : MonoBehaviour {
    void Update()
    {
        //MoveScene(s => s is GameLoginState, "LoginScene");
        MoveScene(s => s is GameLobbyState, "RoomListScene");
        MoveScene(s => s is GameRoomState, "RoomScene");
    }

    void MoveScene(Predicate<GameState> predicate, string target) {
        if (predicate.Invoke(ChessClientManager.UnsafeClient?.State)) {
            if (SceneManager.GetActiveScene().name != target)
                SceneManager.LoadScene(target);
        }
    }
}