using System;
using EndoAshu.Chess.Client.State;
using UnityEngine;
using UnityEngine.UI;

public class ChatSendBehaviour : MonoBehaviour {
    public TMPro.TMP_InputField messageField;
    public Button btn;

    void Start()
    {
        messageField.onSubmit.AddListener(OnSubmitMessage);
        btn.onClick.AddListener(OnSendClick);
    }

    private void OnSendClick()
    {
        OnSubmitMessage(messageField.text);
    }

    void Oestroy()
    {
        messageField.onSubmit.RemoveListener(OnSubmitMessage);
        btn.onClick.RemoveListener(OnSendClick);
    }

    private void OnSubmitMessage(string arg0)
    {
        if (string.IsNullOrEmpty(arg0)) return;
        if (ChessClientManager.Client.State is GameLobbyState ls) {
            ls.SendChat(arg0);
            messageField.ActivateInputField();
        }
        messageField.text = string.Empty;
    }
}