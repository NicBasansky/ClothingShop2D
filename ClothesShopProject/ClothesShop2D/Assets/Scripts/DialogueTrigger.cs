using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shop.Control;

namespace Shop.UI
{
    public class DialogueTrigger : MonoBehaviour
    {
        public Dialogue dialogue;
        bool hasShop = false;
        Store store;

        void Start()
        {
            store = GetComponent<Store>();
            
        }

        public void TriggerDialogue(Vector2 playerPos)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue, store);

            NPC_Controller aiController = GetComponent<NPC_Controller>();
            if (aiController == null) return;

            aiController.FacePlayer(playerPos);
        }
    }

}