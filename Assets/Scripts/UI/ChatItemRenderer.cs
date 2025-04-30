using EndoAshu.Chess.Client.State;
using UnityEngine;

public class ChatItemRenderer : MonoBehaviour
{
    public TMPro.TMP_Text NameField;
    public TMPro.TMP_Text MessageField;

    public void SetItem(GameLobbyState.ChatItem t)
    {
        NameField.text = $"<{t.Name}>";
        MessageField.text = t.Message;
    }
}