using System.Threading.Tasks;
using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.User;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginBtnBehaviour : MonoBehaviour {

    public TMPro.TMP_InputField ID;
    public TMPro.TMP_InputField PW;
    public TMPro.TMP_Text errorText;

    private Button button;
    
    #nullable enable
    internal UnityTaskResult<LoginPacket.LoginStatus>? LoginResponse;
#nullable disable

    void Start()
    {
        button = GetComponent<Button>();
        errorText.text = "";
    }

    void Update()
    {
        ID.interactable = LoginResponse == null;
        PW.interactable = LoginResponse == null;
        button.interactable = LoginResponse == null;
        if (LoginResponse != null) {
            if (LoginResponse.HasResult) {
                var result = LoginResponse.Result;
                LoginResponse = null;
                if (result == LoginPacket.LoginStatus.SUCCESS) {
                    SceneManager.LoadScene("RoomListScene");
                } else {
                    switch(result) {
                        case LoginPacket.LoginStatus.ALREADY_LOGINED:
                            SceneManager.LoadScene("RoomListScene");
                            break;
                        case LoginPacket.LoginStatus.FAILED:
                            errorText.text = "Login Failed... Check for ID or Password.";
                            break;
                        case LoginPacket.LoginStatus.PROTOCOL_MISMATCH:
                            errorText.text = "Login Failed... Check for Game Version!";
                            break;
                        case LoginPacket.LoginStatus.TIMEOUT:
                            errorText.text = "Server is Maintenance!";
                            break;
                    }
                }
            }
        }
    }

    public void OnClickLogin() {
        if (LoginResponse != null) return;
        string inputID = ID.text;
        string inputPW = PW.text;
        if (string.IsNullOrEmpty(inputID)) {
            errorText.text = "Please input ID!";
            return;
        }
        if (string.IsNullOrEmpty(inputPW)) {
            errorText.text = "Please input Password!";
            return;
        }
        errorText.text = "";
        LoginResponse = new UnityTaskResult<LoginPacket.LoginStatus>();
        Task.Run(async () => {
            try {
                var state = ChessClientManager.Client.State as GameLoginState;
                state!.__InternalResetLogin();
                var res = await state!.Login(inputID, inputPW, 5000);
                LoginResponse?.SetResult(res);
            } catch {
                LoginResponse?.SetResult(LoginPacket.LoginStatus.TIMEOUT);
            }
        });
    }
}