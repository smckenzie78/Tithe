using TMPro;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public bool Busy = false;
    PlayerMovement playerMovement;
    PlayerActions playerActions;
    DialogueController dialogueController;
    ChoiceController choiceController;
    void Awake()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerActions = gameObject.GetComponent<PlayerActions>();
        dialogueController = FindAnyObjectByType<DialogueController>();
        choiceController = FindAnyObjectByType<ChoiceController>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement.enabled = !Busy;
        playerActions.enabled = !Busy;
    }
}
