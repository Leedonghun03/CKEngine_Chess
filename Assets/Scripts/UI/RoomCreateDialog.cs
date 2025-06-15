using System.Threading.Tasks;
using EndoAshu.Chess.Client.Room;
using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Room;
using Runetide.Util;
using UnityEngine;
using UnityEngine.UI;

public class RoomCreateDialog : MonoBehaviour
{
    public TMPro.TMP_InputField nameField;
    public TMPro.TMP_InputField passwordField;
    private RoomOptions opt = new RoomOptions();
    public GameObject[] activeTargets;

    public Button okBtn;
    public Button closeBtn;

    void Start()
    {
        opt.Mode = GameMode.ONE_VS_ONE;
        DialogManager.showRoomCreate = false;
        OnClose();
    }

    bool latest = false;

    void Update()
    {
        if (latest != DialogManager.showRoomCreate) {
            latest = DialogManager.showRoomCreate;
            foreach(var t in activeTargets)
                t.SetActive(latest);
            }
        if (!DialogManager.showRoomCreate) return;
        opt.Name = nameField.text;
        opt.Password = string.IsNullOrEmpty(passwordField.text) ? null : passwordField.text;
        okBtn.interactable = okBtn.enabled = opt.CheckAllowed();
    }

    public void OnClickOk() {
        if (nameField.text.Length >= 2) {
            Debug.Log(ChessClientManager.UnsafeClient?.State);
            if (ChessClientManager.UnsafeClient?.State is GameLobbyState ls) {
                //ChessClientManager.UnsafeClient?.Send(new ClientSideRoomCreatePacket.Request(opt));
                //ls.IUnderstandThisMethodIsNotRecommendButWantResetRoomCreateResponse();
                ls.CreateRoom(opt, 3000).Then((stat) => {
                    var status = stat.Item1;
                    var uuid = stat.Item2;
                    Debug.Log(status);
                    Debug.Log(uuid);
                    if (status == RoomCreatePacket.CreateStatus.SUCCESS || status == RoomCreatePacket.CreateStatus.ALREADY_INSIDE) {
                    } else {
                        //TODO : 임시로 TIMEOUT 응답은 dialog 미표출 - 추후 버그 해결 시 제거
                        if (status != RoomCreatePacket.CreateStatus.TIMEOUT)
                            DialogManager.Current = new DialogManager.Instance("Failed", $"Failed to create room.\nReason : {status}", "Close", () => {});
                    }
                });
            }
            OnClose();
        }
    }

    public void OnClickClose() {
        OnClose();
    }

    private void OnClose() {
        DialogManager.showRoomCreate = false;
        nameField.text = "";
        passwordField.text = "";
        opt.Name = string.Empty;
        opt.Password = null;
    }
}
