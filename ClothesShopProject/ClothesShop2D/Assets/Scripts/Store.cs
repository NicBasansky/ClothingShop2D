using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shop.UI;
using System;
using UnityEngine.Playables;

[System.Serializable]
public class Outfit
{
    public AnimatorOverrideController animatorOverrideController;
    public Sprite idleDownSprite;
    public float cost = 5.0f;
    public bool previouslyPurchased = false;
}

public class Store : MonoBehaviour
{
    [SerializeField] Outfit[] outfits;
    [SerializeField] SpriteRenderer playerSpriteRenderer;
    [SerializeField] ShopUI shopUI;
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] PlayableDirector playableDirector;
    Sprite originalSprite = null;
    Animator originalAnimator = null;
    Animator animator;
    Outfit purchased = null;



    void Start()
    {
        
        originalSprite = playerSpriteRenderer.sprite;
        animator = playerSpriteRenderer.GetComponent<Animator>();
        originalAnimator = animator;
    }

    public void OpenShop()
    {
        shopUI.Init(this);
    }

    public Outfit[] GetOutfits()
    {
        return outfits;
    }

    public Sprite GetOriginalSprite()
    {
        return originalSprite;
    }

    public SpriteRenderer GetPlayerSpriteRenderer()
    {
        return playerSpriteRenderer;
    }

    IEnumerator SetNewOutfit()
    {
        yield return new WaitForSeconds(8.5f);

        animator.runtimeAnimatorController = purchased.animatorOverrideController;
        originalSprite = playerSpriteRenderer.sprite;

        // This is to make the character facing the camera once the door opens
        animator.SetFloat("Vertical", 1.0f); 
        animator.SetFloat("Vertical", 0);

    }

    public void PrepareNewOutfit(Outfit outfit)
    {
        purchased = outfit;
        StartCoroutine(SetNewOutfit());
    }

    public void StartTimeline()
    {
        playableDirector.Play();
    }


}
