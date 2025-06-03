using System;
using UnityEngine;
using UnityEngine.UI;

public enum TypeId
{
    PAWN,
    KNIGHT,
    ROOK,
    BISHOP,
    QUEEN,
    KING,
    CANCELLED,
}

public class PromotionUIButton : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button queenButton;
    [SerializeField] private Button knightButton;
    [SerializeField] private Button rookButton;
    [SerializeField] private Button bishopButton;

    public event Action<TypeId> OnPromotionSelected;

    private void Awake()
    {
        if (!backButton)
        {
            backButton.onClick.AddListener(OnBackButtonClick);
        }

        if (!queenButton)
        {
            queenButton.onClick.AddListener(OnQueenButtonClick);
        }

        if (!knightButton)
        {
            knightButton.onClick.AddListener(OnKnightButtonClick);
        }

        if (!rookButton)
        {
            rookButton.onClick.AddListener(OnRookButtonClick);
        }

        if (!bishopButton)
        {
            bishopButton.onClick.AddListener(OnBishopButtonClick);
        }
    }

    public void OnBackButtonClick()
    {
        HandlePromotionButtonClicked(TypeId.CANCELLED);
    }

    public void OnQueenButtonClick()
    {
        HandlePromotionButtonClicked(TypeId.QUEEN);
    }

    public void OnKnightButtonClick()
    {
        HandlePromotionButtonClicked(TypeId.KNIGHT);
    }

    public void OnRookButtonClick()
    {
        HandlePromotionButtonClicked(TypeId.ROOK);
    }

    public void OnBishopButtonClick()
    {
        HandlePromotionButtonClicked(TypeId.BISHOP);
    }
    
    private void OnDestroy()
    {
        if (!backButton)
        {
            backButton.onClick.RemoveListener(OnBackButtonClick);
        }

        if (!queenButton)
        {
            queenButton.onClick.RemoveListener(OnQueenButtonClick);
        }

        if (!knightButton)
        {
            knightButton.onClick.RemoveListener(OnKnightButtonClick);
        }

        if (!rookButton)
        {
            rookButton.onClick.RemoveListener(OnRookButtonClick);
        }

        if (!bishopButton)
        {
            bishopButton.onClick.RemoveListener(OnBishopButtonClick);
        }
    }

    private void HandlePromotionButtonClicked(TypeId type)
    {
        if (OnPromotionSelected != null)
        {
            OnPromotionSelected.Invoke(type);
        }
    }
}
