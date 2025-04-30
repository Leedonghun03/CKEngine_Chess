using System;
using System.Collections;
using EndoAshu.Chess.Client.State;
using UnityEngine;
using UnityEngine.UI;

public class ChatRenderer : MonoBehaviour
{
    public ScrollRect ScrollRect;
    public Transform ChatContent;
    public GameObject ChatItemPrefab;

    void Start()
    {
        if (ChessClientManager.Client.State is GameLobbyState ls) {
            ls.ChatReceived += OnChatReceived;
        }
    }

    void OnDestroy()
    {
        if (ChessClientManager.Client.State is GameLobbyState ls) {
            ls.ChatReceived -= OnChatReceived;
        }
    }

    private void OnChatReceived(GameLobbyState.ChatItem t)
    {
        GameObject newChatItem = Instantiate(ChatItemPrefab, ChatContent);

#nullable enable
        GameObject? high = null;
        GameObject? low = null;
        for(int i = 0; i < ChatContent.childCount; ++i) {
            GameObject child = ChatContent.GetChild(i).gameObject;
            if (high == null || child.transform.position.y > high.transform.position.y) {
                high = child;
            }
            if (low == null || child.transform.position.y < low.transform.position.y) {
                low = child;
            }
        }

        newChatItem.transform.position = new Vector3(
            newChatItem.transform.position.x,
            low == null ? 0 : low.transform.position.y - newChatItem.GetComponent<RectTransform>().rect.height,
            newChatItem.transform.position.z
        );

        var renderer = newChatItem.GetComponent<ChatItemRenderer>();
        renderer.SetItem(t);
        StartCoroutine(SetScrollTopNextFrame());
#nullable disable
    }

    IEnumerator SetScrollTopNextFrame()
    {
        yield return new WaitForEndOfFrame();
        ScrollRect.verticalNormalizedPosition = 0f;
    }
}
