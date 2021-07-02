using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NicLib.Shops;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Image avatar;
    [SerializeField] Button leftArrow;
    [SerializeField] Button rightArrow;
    [SerializeField] Button buyButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button shirtsButton;
    [SerializeField] Button pantsButton;
    [SerializeField] TextMeshProUGUI costAmount;
    [SerializeField] TextMeshProUGUI walletAmount;
    [SerializeField] Wallet playerWallet;
    Animator playerAnimator;
    Store store;
    Outfit[] outfits;
    int outfitIndex = 0;
    PurchaseClothesSequencer sequencer;

    void Start()
    {
        costAmount.text = "0.00";
        panel.SetActive(false);
        leftArrow.onClick.AddListener(LeftArrow);
        rightArrow.onClick.AddListener(RightArrow);
        buyButton.onClick.AddListener(Buy);
        cancelButton.onClick.AddListener(Cancel);
        // shirtsButton.onClick.AddListener(ChooseFromShirts);
        // pantsButton.onClick.AddListener(ChooseFromPants);
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        
    }

    public void Init(Store store)
    {
        this.store = store;
        outfits = store.GetOutfits();
        SetActiveShopUI(true);
        outfitIndex = 0;
        walletAmount.text = string.Format("{0:0.00}", playerWallet.GetTotalMoney());
        UpdateSprite();
    }

    private void SetActiveShopUI(bool enabled)
    {
        panel.SetActive(enabled);
    }


    private void LeftArrow()
    {
        --outfitIndex;
        if (outfitIndex < 0)
        {
            outfitIndex = outfits.Length - 1;
        }
        UpdateSprite();
    }

    private void RightArrow()
    {
        ++outfitIndex;
        if (outfitIndex >= outfits.Length)
        {
            outfitIndex = 0;
        }
        UpdateSprite();
    }

    void UpdateSprite()
    {
        avatar.sprite = outfits[outfitIndex].idleDownSprite;
        costAmount.text = string.Format("{0:0.00}", outfits[outfitIndex].cost);
    }

    private void Buy()
    {
        if (outfits[outfitIndex].previouslyPurchased || !playerWallet.CheckIfHasEnoughMoney(outfits[outfitIndex].cost))
            return;
        outfits[outfitIndex].previouslyPurchased = true;

        // sequencer.BeginSequence();
        //store.SetNewOutfit(outfits[outfitIndex]);

        // change outfits after moving to change room
        //if (outfits[outfitIndex].animatorOverrideController)
        //   playerAnimator.runtimeAnimatorController = outfits[outfitIndex].animatorOverrideController;
        store.StartTimeline();
        playerWallet.SubtractAmount(outfits[outfitIndex].cost);
        walletAmount.text = string.Format("{0:0.00}", playerWallet.GetTotalMoney());
        store.PrepareNewOutfit(outfits[outfitIndex]);
        SetActiveShopUI(false);
    }

    private void Cancel()
    {
        avatar.sprite = store.GetOriginalSprite();
        SetActiveShopUI(false);
    }

}
