using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NicLib.GridPlacement;
using TMPro;
using UnityEngine.UI;

public class PurchaseableItemUI : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Image displaySprite;
    [SerializeField] TMP_Text text;

    void Start()
    {
        Placeable placeable = prefab.GetComponent<Placeable>();
        if (placeable == null)
        {
            Debug.Log("Purchasable Item doesn't have the Placeable script on it!");
            return;
        }

        displaySprite.sprite = placeable.GetFrontSprite();
        displaySprite.SetNativeSize();
        text.text = string.Format("${0:N2}", placeable.GetItemCost().ToString());
    }

    public void SpawnPrefab()
    {
        GridSystem.current.Initialize(prefab);
    }
}
