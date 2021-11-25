using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NicLib.Shops;

public class PlaceableUI : MonoBehaviour
{
    [SerializeField] TMP_Text walletText;
    Wallet wallet;

    void Awake()
    {
        wallet = GameObject.FindGameObjectWithTag("Player").GetComponent<Wallet>();
        wallet.onChange += UpdateWalletText;
    }

    void Start()
    {
        UpdateWalletText();
    }

    public void UpdateWalletText()
    {
        walletText.text = string.Format("${0:N0}", wallet.GetTotalMoney());
    }
}
