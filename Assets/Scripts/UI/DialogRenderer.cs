using System;
using Runetide.Util;
using UnityEngine;

public class DialogRenderer : MonoBehaviour
{
    public TMPro.TMP_Text Title;
    public TMPro.TMP_Text Message;
    public TMPro.TMP_Text ConfirmBtn;
    private UUID latest = UUID.NULL;
    public GameObject[] activeTargets;

    void Start()
    {
        foreach(var t in activeTargets)
            t.SetActive(false);
    }

    void Update()
    {
        UUID current = DialogManager.Current?.UniqueId ?? UUID.NULL;
        if (!latest.Equals(current)) {
            latest = current;
            var instance = DialogManager.Current;
            if (instance == null) {
                foreach(var t in activeTargets)
                    t.SetActive(false);
            } else {
                Title.text = instance.Title;
                Message.text = instance.Message;
                ConfirmBtn.text = instance.ConfirmText;
                foreach(var t in activeTargets)
                    t.SetActive(true);
            }
        }
    }
    
    public void OnClick() {
        DialogManager.Current?.OnClick?.Invoke();
        DialogManager.Current = null;
    }
}
