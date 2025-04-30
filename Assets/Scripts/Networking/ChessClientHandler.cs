using UnityEngine;
using UnityEngine.SceneManagement;

public class ChessClientHandler : MonoBehaviour {
    private static readonly DialogManager.Instance DisconnDialog = new DialogManager.Instance("Disconnect", "Network is missing...\nReturn to Login Page", "Ok", () => SceneManager.LoadScene("LoginScene"));

    void Update()
    {
        var client = ChessClientManager.UnsafeClient;
        if (client != null && client.Account != null) {
            if (DialogManager.Current != null && DialogManager.Current.UniqueId == DisconnDialog.UniqueId) {
                DialogManager.Current = null;
            }
            client.OnTick();
        } else {
            if (DialogManager.Current == null || DialogManager.Current.UniqueId != DisconnDialog.UniqueId) {
                DialogManager.Current = DisconnDialog;
            }
        }
    }
}