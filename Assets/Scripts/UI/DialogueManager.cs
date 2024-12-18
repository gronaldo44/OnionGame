using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    private TextAsset textFile;
    private string[] linesOfDialogue;
    private int dialogueIndex;

    public bool dialogueIsPlaying { get; private set; }

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one DialogueManager");
        }

        instance = this;
        linesOfDialogue = new string[0];
        dialogueIndex = 0;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
    }

    public void EnterDialogueMode(TextAsset fullDialogueText)
    {
        if (fullDialogueText == null)
        {
            Debug.LogWarning("Dialogue text is null.");
            return;
        }

        textFile = fullDialogueText; // Store reference to the textFile
        linesOfDialogue = fullDialogueText.text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        linesOfDialogue = linesOfDialogue.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

        // Set the NPC name (first line of the text)
        Transform speaker = dialoguePanel.transform.Find("SpeakerName");
        if (speaker != null)
        {
            TextMeshProUGUI speakerText = speaker.GetComponent<TextMeshProUGUI>();
            speakerText.text = linesOfDialogue[0]; // First line is the NPC name
            Debug.Log(speakerText.text);
        }
        else
        {
            Debug.LogError("SpeakerName not found");
        }

        // Start dialogue from the second line (skip the first line for the NPC name)
        dialogueIndex = 1;
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueDialogue();
    }

    public void ExitDialogueMode()
    {
        Debug.Log("Exit dialogue mode");
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        dialogueIsPlaying = false;
        dialogueIndex = 0;

        textFile = null; // Clear the textFile reference after dialogue ends
    }

    public void ContinueDialogue()
    {
        Debug.Log("dialogue index: " + dialogueIndex + "/" + linesOfDialogue.Length);
        if (dialogueIndex < linesOfDialogue.Length)
        {
            dialogueText.text = linesOfDialogue[dialogueIndex++];
        }
        else
        {
            ExitDialogueMode();
        }
    }
}