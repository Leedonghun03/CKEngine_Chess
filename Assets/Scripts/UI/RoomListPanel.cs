using EndoAshu.Chess.Room;
using UnityEngine;

public class RoomListPanel : MonoBehaviour
{
    #nullable enable
    public RoomListPacket.Item? data;
    #nullable disable

    public TMPro.TMP_Text titleText;
    public TMPro.TMP_Text memberText;
    
    void Start()
    {
        
    }


    void Update()
    {
        titleText.text = data != null ? data.RoomName : "";
        memberText.text = data != null ? data.MasterName : "";
    }
}
