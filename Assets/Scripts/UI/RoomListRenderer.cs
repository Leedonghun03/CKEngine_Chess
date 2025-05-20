using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using EndoAshu.Chess.Client.State;
using EndoAshu.Chess.Room;
using Runetide.Util;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoomListRenderer : MonoBehaviour {

    public int CurrentPage = 1;
    public int QueryPage = 1;
    public int TotalPage = 1;
    public ReadOnlyCollection<RoomListPacket.Item> Items = new List<RoomListPacket.Item>().AsReadOnly();
    public int ItemsPerPage = 5;

    private float updateTicks = 0.0f;

    public InputAction LeftRoom;
    public InputAction RightRoom;

    public Transform RoomListTransform;
    public GameObject RoomUIPrefab;

    private void Start()
    {
        if (LeftRoom != null)
            LeftRoom.performed += MoveLeftRoom;
        if (RightRoom != null)
            RightRoom.performed += MoveRightRoom;
    }

    private void MoveLeftRoom(InputAction.CallbackContext context)
    {
        --QueryPage;
        updateTicks = 0.0f;
    }

    private void MoveRightRoom(InputAction.CallbackContext context)
    {
        ++QueryPage;
        updateTicks = 0.0f;
    }

    public void Update()
    {
        updateTicks -= Time.deltaTime;
        if (updateTicks < 0.0f) {
            updateTicks = 99.0f;
            RefreshRoomList(QueryPage).Then((e) => {
                if (TotalPage == 1 && CurrentPage > 1) {
                    QueryPage = 1;
                    updateTicks = 0.0f;
                }
                
                for(int i = 0; i < RoomListTransform.childCount; ++i) {
                    RoomListTransform.GetChild(i).gameObject.SetActive(false);
                }

                for(int i = RoomListTransform.childCount; i <= Items.Count; ++i) {
                    Instantiate(RoomUIPrefab, RoomListTransform);
                }
                
                for(int i = 0; i < Items.Count; ++i) {
                    var child = RoomListTransform.GetChild(i);
                    var rect = child.GetComponent<RectTransform>();
                    rect.localPosition = new Vector2(16, -16 + -184 * i);
                    child.GetComponent<RoomListPanel>().data = Items[i];
                    child.gameObject.SetActive(true);
                }
            }).Catch((e) => {
            }).Finally(() => {
                updateTicks = 1.0f;
            });
        }
    }

    private async Task<bool> RefreshRoomList(int page) {
        if (ChessClientManager.UnsafeClient?.State is GameLobbyState ls) {
            var res = await ls.RoomList(page, ItemsPerPage);
            if (res.Status == RoomListPacket.DataStatus.SUCCESS) {
                Items = res.Items;
                TotalPage = res.TotalPages;
                CurrentPage = res.Page;
                return true;
            }
            ChessClientManager.Client.Logger.Warn($"Refresh Room failed : {res.Status}");
            return false;
        }
        return false;
    }
}