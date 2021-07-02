using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shop.UI;
using System;
using UnityEngine.Playables;
using Shop.Control;

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
    [SerializeField] ShopUI shopUI;
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] PlayableDirector playerPlayableDirector;
    [SerializeField] PlayableDirector enviroPlayableDirector;

    float changeOutfitDelaySeconds = 7.5f;
    float enviroTimelineDelay = 8.25f;
    Animator animator;
    Outfit purchased = null;


    // todo block being able to talk to people if shopping
    void Start()
    {
        animator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    public void OpenShop()
    {
        shopUI.Init(this);
    }

    public Outfit[] GetOutfits()
    {
        return outfits;
    }
    
    IEnumerator SetNewOutfit()
    {
        yield return new WaitForSeconds(changeOutfitDelaySeconds);

        animator.runtimeAnimatorController = purchased.animatorOverrideController;
        animator.GetComponent<PlayerController>().shouldFreeze = false;
        animator.GetComponent<PlayerInteracter>().SetIsShopOpen(false);

        // This is to make the character facing the camera once the door opens
        animator.SetFloat("Vertical", -1.0f); 
        //animator.SetFloat("Vertical", 0);
    }

    public void PrepareNewOutfit(Outfit outfit)
    {
        purchased = outfit;
        StartCoroutine(SetNewOutfit());
        StartCoroutine(StartTimeline());
    }

    private IEnumerator StartTimeline()
    {
        playerPlayableDirector.Play();
        yield return new WaitForSeconds(enviroTimelineDelay);
        enviroPlayableDirector.Play();
    }
}
