using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shop.UI;

namespace Shop.Control
{
    public class PlayerInteracter : MonoBehaviour
    {
        public GameObject currentInteractable = null;

        [SerializeField] DialogueManager dialogueManager = null;

        private PlayerController playerController;
        private bool interacting = false;
        private bool dialogueTriggered = false;



        private void OnEnable()
        {
            dialogueManager.onDialogueEnd += OnEndDialogue;
        }

        private void OnDisable()
        {
            dialogueManager.onDialogueEnd -= OnEndDialogue;
        }

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
        }

        // could have issues with multiple overlapping triggers
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Interactable"))
            {
                currentInteractable = other.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject == currentInteractable)
            {
                currentInteractable = null;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)
                             && currentInteractable != null)
            {
                Interact();
            }
        }

        private void Interact()
        {
            if (!interacting)
            {
                interacting = true;

                currentInteractable.GetComponent<DialogueTrigger>().TriggerDialogue(transform.position);
                dialogueTriggered = true;
                playerController.shouldFreeze = true;
            }
            else if (dialogueTriggered)
            {
                dialogueManager.DisplayNextSentence();
            }
        }

        private void OnEndDialogue() // TODO freeze movement
        {
            interacting = false;
            dialogueTriggered = false;
            playerController.shouldFreeze = false;
        }
    }
}
