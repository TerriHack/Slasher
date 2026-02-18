using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject dialogueBubble;

    [SerializeField] private float bubbleDuration;
    [SerializeField] private float bubbleCooldown;
    private float cooldownTimer;

    [SerializeField] private List<Sprite> bubbleSprites;
    [SerializeField] private List<string> phaseZeroDialogues;
    [SerializeField] private List<string> fleeDialogues;
    [SerializeField] private List<string> attackDialogues;
    [SerializeField] private List<string> isHeGoneDialogues;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public bool SpawnDialogueBubble(GameObject ai, int typeOfDialogue)
    {
        if (cooldownTimer > 0) return false;
        
        var targetScreenPos = Camera.main.WorldToScreenPoint(ai.transform.position);
        
        var bubble = Instantiate(dialogueBubble, targetScreenPos, Quaternion.identity, canvas.transform);

        var r = 0;
        var text = "";
        
        switch (typeOfDialogue)
        {
            case 0:
                r = Random.Range(0, phaseZeroDialogues.Count);
                text = phaseZeroDialogues[r];
                break;
            case 1:
                r = Random.Range(0, fleeDialogues.Count);
                text = fleeDialogues[r];
                break;
            case 2:
                r = Random.Range(0, attackDialogues.Count);
                text = attackDialogues[r];
                break;
            case 3:
                r = Random.Range(0, isHeGoneDialogues.Count);
                text = isHeGoneDialogues[r];
                break;
        }
        
        bubble.GetComponent<DialogueBubble>().InitBubble(bubbleSprites[typeOfDialogue], text, ai);

        cooldownTimer = bubbleCooldown;

        Destroy(bubble, bubbleDuration);
        
        return true;
    }
}
