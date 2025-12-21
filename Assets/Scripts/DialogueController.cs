using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private Canvas canvas;

    private List<string> lines;
    private int index;
    private bool active;

    private PlayerState playerState;

    void Awake()
    {
        playerState = FindAnyObjectByType<PlayerState>();
        canvas.enabled = false;
    }

    void Update()
    {
        if (!active) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Advance();
        }
    }

    public void StartDialogue(List<string> dialogueLines)
    {
        if (dialogueLines == null || dialogueLines.Count == 0)
            return;

        lines = dialogueLines;
        index = 0;
        active = true;

        playerState.Busy = true;
        canvas.enabled = true;
        textComponent.text = lines[index];
    }

    private void Advance()
    {
        index++;

        if (index >= lines.Count)
        {
            EndDialogue();
        }
        else
        {
            textComponent.text = lines[index];
        }
    }

    private void EndDialogue()
    {
        active = false;
        canvas.enabled = false;
        lines = null;
        index = 0;

        playerState.Busy = false;
    }

    public bool IsActive()
    {
        return active;
    }
}
