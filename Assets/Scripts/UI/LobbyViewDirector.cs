using System.Threading.Tasks;
using EndoAshu.Chess.Client.State;

public class LobbyViewDirector {
    const int ITEMS_PER_PAGE = 6;

    public async Task QueryRoom(int page) {
        if (ChessClientManager.UnsafeClient?.State is GameLobbyState ls) {
            await ls.RoomList(page, ITEMS_PER_PAGE);
        }
    }
}