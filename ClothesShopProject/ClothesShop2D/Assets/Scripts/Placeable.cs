using System;
using UnityEngine;

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


        [HideInInspector]
        public bool placed;
        [HideInInspector]
        public bool purchased = false;
        [HideInInspector]
        public bool isSelected = false;
        [HideInInspector]
        public bool isMoving = false;

        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] GameObject front;
        [SerializeField] GameObject left;
        [SerializeField] GameObject back;
        [SerializeField] GameObject right;

        // [SerializeField] Sprite front;
        // [SerializeField] Sprite left;
        // [SerializeField] Sprite back;
        // [SerializeField] Sprite right;
        //[SerializeField] Vector3 rotatedSpriteOffset;
        [SerializeField] BoxCollider2D originalCollider;
        [SerializeField] BoxCollider2D rotatedCollider;
        // private Vector3 originalOffset;
        public SpriteFaceDirection faceDirection = SpriteFaceDirection.Front;
        private Collider2D colliderToUse;

        void Awake()
        {
            colliderToUse = originalCollider;
            originalCollider.enabled = false;
            rotatedCollider.enabled = false;
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
     
        }


        public bool CanBePlaced()
        {
            return GridSystem.current.CheckIfTilesAreAvailable(areaUnderneath);
        }

        public void Place()
        {
            // set the tiles on main tilemap to those on the temp in the area underneath placeable object
            placed = true;
            GridSystem.current.TakeArea(areaUnderneath);
            EnableColliderToUse();

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
            if (isMoving) return;
            if (GridSystem.current.CanAcceptPlaceableObject())
            {
                if (GridSystem.current.IsDecoMode())
                {
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
            print(faceDirection);

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

    }
}