using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NicLib.Shops
{
    public class Wallet : MonoBehaviour
    {
        [SerializeField] float money = 20.0f;

        //public delegate void OnChange(float newTotal);
        public event Action onChange;

        public void AddAmount(float amount)
        {
            money += amount;
            if (onChange != null)
            {
                onChange();
            }
        }

        public void SubtractAmount(float amount)
        {
            money -= amount;
            money = Mathf.Max(0, money);

            if (onChange != null)
            {
                onChange();
            }
        }

        public bool CheckIfHasEnoughMoney(float amount)
        {
            if (money - amount < 0)
                return false;
            return true;
        }

        public float GetTotalMoney()
        {
            return money;
        }
    }

}