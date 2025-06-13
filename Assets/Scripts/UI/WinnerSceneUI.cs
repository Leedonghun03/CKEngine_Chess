using UnityEngine;

public class WinnerSceneUI : MonoBehaviour
{
    public TMPro.TMP_Text winnerText;

    void Start()
    {

    }

    void Update()
    {
        var dat = ChessClientManager.GameEndData;
        if (dat != null) {
            winnerText.text = dat.WinnerTeam == EndoAshu.Chess.Room.PlayerMode.TEAM2 ? "Black Win" : "White Win";
        }
    }

    public void OnReturnRoom()
    {
        ChessClientManager.GameEndData = null;
    }
}
