using UnityEngine;

public class RoomInnerSimple : MonoBehaviour
{
    public TMPro.TMP_Text Info;

    void Start()
    {
        
    }

    void Update()
    {
        var room = ChessClientManager.UnsafeClient?.CurrentRoom;        
        if (room != null) {
            string str = $"Members : {room.Members.Count}\n";
            foreach(var i in room.Members) {
                str += $"\n[{i.Mode}] {i.Name}";
            }
            Info.text = str;
        }
    }
}
