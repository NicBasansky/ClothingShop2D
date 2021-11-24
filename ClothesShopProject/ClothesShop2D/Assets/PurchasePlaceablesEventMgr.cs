using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// We'll use this to purchase the furniture
public class PurchasePlaceablesEventMgr : MonoBehaviour
{
    // Singleton
    private static PurchasePlaceablesEventMgr _instance;

    public static PurchasePlaceablesEventMgr Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }else {
            _instance = this;
        }
    }

    public event Action onMoveRequest;


    
    

}