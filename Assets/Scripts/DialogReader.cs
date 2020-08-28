﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogReader : MonoBehaviour
{
    [SerializeField] DialogInterface dialogUI = null;
    [SerializeField] PlayerMovement movement = null;

    Conversation currentConversation = null;
    int conversationIndex = 0;

    PlayerInputActions controls;

    DialogNode currentNode = null;

    void Awake()
    {
        controls = new PlayerInputActions();
        controls.Player.Interact.performed += Interact;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Interact(InputAction.CallbackContext context)
    {
        if(currentConversation != null)
        {
            conversationIndex++;
            if(conversationIndex >= currentConversation.nodes.Count)
            {
                currentConversation = null;
                dialogUI.CloseDialog();
                movement.EndAnimation();
            } 
            else
            {
                dialogUI.ShowDialog(currentConversation.nodes[conversationIndex]);
                movement.StartAnimation(currentConversation.nodes[conversationIndex].playerAnimation);
            }
        }
        else if(currentNode != null)
        {
            currentConversation = currentNode.GetConversation();
            conversationIndex = 0;
            dialogUI.ShowDialog(currentConversation.nodes[conversationIndex]);
            movement.StartAnimation(currentConversation.nodes[conversationIndex].playerAnimation, 
                currentNode.TurnToFace ? currentNode.transform : null);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        DialogNode dialog = other.GetComponent<DialogNode>();
        if(dialog != null && dialog != currentNode)
        {
            if(currentNode != null)
            {
                currentNode.SetVisible(false);
            }
            currentNode = dialog;
            currentNode.SetVisible(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DialogNode dialog = other.GetComponent<DialogNode>();
        if (dialog == currentNode && currentNode != null)
        {
            currentNode.SetVisible(false);
            currentNode = null;
        }
    }
}
