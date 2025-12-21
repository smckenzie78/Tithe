using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    public NPCData npcData;
    DialogueController dialogueController;
    ChoiceController choiceController;
    void Awake()
    {
        dialogueController = FindAnyObjectByType<DialogueController>();
        choiceController = FindAnyObjectByType<ChoiceController>();
    }

    void Update()
    {

    }

    public void NPCInteraction()
    {
        dialogueController.StartDialogue(new List<string> {npcData.baseDialogue[Random.Range(0, npcData.baseDialogue.Count)]});
    }
}
