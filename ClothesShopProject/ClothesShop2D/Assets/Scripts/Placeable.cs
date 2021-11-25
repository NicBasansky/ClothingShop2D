using System;
using UnityEngine;
using NicLib.Shops;

namespace NicLib.GridPlacement
{
    public enum SpriteFaceDirection
    {
        Front,
        Left,
        Back,
        Right
    };
    public class Placeable : MonoBehaviour
    {
        [Tooltip("z value must be at least 1")]
        public BoundsInt areaUnderneath;
        private BoundsInt originalArea;

        [SerializeField] int price = 0;


        //[HideInInspector]
        public bool placed;
        //[HideInInspector]
        public bool purchased = false;
        //[HideInInspector]
        public bool isSelected = false;
        //[HideInInspector]
        public bool isMoving = false;

        [SerializeField] GameObject front;
        [SerializeField] GameObject left;
        [SerializeField] GameObject back;
        [SerializeField] GameObject right;
        [SerializeField] BoxCollider2D originalCollider;
        [SerializeField] BoxCollider2D rotatedCollider;



        public SpriteFaceDirection faceDirection = SpriteFaceDirection.Front;
        private Collider2D colliderToUse;
        private Wallet playerWallet;
        private SpriteRenderer[] spriteRenderers;

        void Awake()
        {
            colliderToUse = originalCollider;
            originalCollider.enabled = false;
            rotatedCollider.enabled = false;

            playerWallet = GameObject.FindGameObjectWithTag("Player").GetComponent<Wallet>();
        }

        void Start()
        {
            

            if (faceDirection == SpriteFaceDirection.Front || faceDirection == SpriteFaceDirection.Back)
            {
                originalArea = areaUnderneath;
            }
            else
            {
                originalArea = new BoundsInt(areaUnderneath.position, new Vector3Int(areaUnderneath.size.y,
                                                    areaUnderneath.size.x, areaUnderneath.size.z));
            }

            if (front == null || left == null || back == null || right == null) return;
            spriteRenderers = new SpriteRenderer[4];
            spriteRenderers[0] = front.GetComponent<SpriteRenderer>();
            spriteRenderers[1] = back.GetComponent<SpriteRenderer>();
            spriteRenderers[2] = left.GetComponent<SpriteRenderer>();
            spriteRenderers[3] = right.GetComponent<SpriteRenderer>();
        }

        public bool CanBePlaced()
        {
            // check if enough money in wallet
            if ((playerWallet.CheckIfHasEnoughMoney(price) && !purchased) || purchased)
            {
                if (GridSystem.current.CheckIfTilesAreAvailable(areaUnderneath))
                {
                    return true;
                }
            }
            else if (!purchased && !playerWallet.CheckIfHasEnoughMoney(price))
            {
                // make a sound to indicate not enough money
                
            }

            return false;
        }

        public void SetOrderLayer()
        {
            foreach(var rend in spriteRenderers)
            {
                rend.sortingOrder = areaUnderneath.position.y * -10;
            }
        }

        public bool CanAfford()
        {
            return playerWallet.CheckIfHasEnoughMoney(price);
        }

        public void Place()
        {
            // set the tiles on main tilemap to those on the temp in the area underneath placeable object
            placed = true;
            GridSystem.current.TakeArea(areaUnderneath);
            EnableColliderToUse();
            
            if (!purchased)
            {
                playerWallet.SubtractAmount(price);
                purchased = true;
            }
        }

        private void EnableColliderToUse()
        {
            if (colliderToUse == originalCollider)
            {
                originalCollider.enabled = true;
                rotatedCollider.enabled = false;
            }
            else
            {
                originalCollider.enabled = false;
                rotatedCollider.enabled = true;
            }
        }

        void OnMouseDown()
        {
            //print("OnMouseDown called by: " + gameObject.name);
            if (isMoving) return;
            if (GridSystem.current.CanAcceptPlaceableObject())
            {
                if (GridSystem.current.IsDecoMode())
                {
                    isMoving = true;
                    colliderToUse.enabled = false;
                    GridSystem.current.MovePlaceable(this.gameObject, this);             
                }
            }
        }

        public void RotateSpriteDirection(bool clockwise)
        {
            if (front == null || left == null || back == null || right == null) return;

            if (clockwise)
            {
                int fd = (int)faceDirection;

                fd++;
                fd %= 4;
                faceDirection = (SpriteFaceDirection)fd;
                //print("fd: " + fd);
            }
            else
            {
                int fd = (int)faceDirection;
                fd--;
                if (fd < 0)
                {
                    fd = 3;
                }
                faceDirection = (SpriteFaceDirection)fd;
            }
            //print(faceDirection + "int: " + ((int)faceDirection));

            EnableDirectionalSprite();
        }

        private void EnableDirectionalSprite()
        {
            if (front == null || left == null || back == null || right == null) return;

            // disable all first
            front.SetActive(false);
            left.SetActive(false);
            back.SetActive(false);
            right.SetActive(false);

            // re-enable the correct sprite
            switch (faceDirection)
            {
                case SpriteFaceDirection.Front:
                    front.SetActive(true);
                    areaUnderneath = originalArea;
                    colliderToUse = originalCollider;
                    break;
                case SpriteFaceDirection.Left:
                    left.SetActive(true);
                    RotateBoundsToSide();
                    colliderToUse = rotatedCollider;
                    break;
                case SpriteFaceDirection.Back:
                    back.SetActive(true);
                    areaUnderneath = originalArea;
                    colliderToUse = originalCollider;
                    break;
                case SpriteFaceDirection.Right:
                    right.SetActive(true);
                    RotateBoundsToSide();
                    colliderToUse = rotatedCollider;
                    break;
            }
        }

        private void RotateBoundsToSide()
        {
            BoundsInt rotatedBounds = new BoundsInt(originalArea.position, new Vector3Int(originalArea.size.y,
                                                originalArea.size.x, originalArea.size.z));
            areaUnderneath = rotatedBounds;
        }

        public int GetItemCost()
        {
            return price;
        }

        public Sprite GetFrontSprite()
        {
            Sprite frontSprite = front.GetComponent<SpriteRenderer>().sprite;
            return frontSprite;
        }

    }
}