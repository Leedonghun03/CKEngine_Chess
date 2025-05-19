using System;
using Runetide.Util;

public static class DialogManager {
#nullable enable
    public class Instance {
        public string Title { get; }
        public string Message { get; }
        public string ConfirmText { get; }
        public Action? OnClick { get; }
        public UUID UniqueId { get; }

        public Instance(string title, string message, string confirmText, Action? onClick) {
            UniqueId = UUID.Create();
            Title = title;
            Message = message;
            ConfirmText = confirmText;
            OnClick = onClick;
        }
    }

    public static Instance? Current { get; set; } = null;

    public static bool showRoomCreate = false;
#nullable disable
}