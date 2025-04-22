using UnityEngine;
using UnityEngine.UI;

public class AccountInfoRenderer : MonoBehaviour
{
    public Image ProfileImage;
    public TMPro.TMP_Text Username;
    public TMPro.TMP_Text Game_Games;
    public TMPro.TMP_Text Game_Win;
    public TMPro.TMP_Text Game_Draw;
    public TMPro.TMP_Text Game_Lose;
    
    void Start()
    {
        
    }

    void Update()
    {
        var account = ChessClientManager.UnsafeClient?.Account;
        if (account != null) {
            Username.text = account.Username;
            Game_Games.text = $"{account.Win + account.Draw + account.Lose} Games";
            Game_Win.text = $"{account.Win} Win";
            Game_Draw.text = $"{account.Draw} Draw";
            Game_Lose.text = $"{account.Lose} Lose";
        }
    }
}
