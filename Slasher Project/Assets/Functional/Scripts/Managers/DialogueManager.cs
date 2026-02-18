using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private Canvas canvas;

    [SerializeField] private float bubbleDuration;
    private float cooldownTimer;

    [Header("Dialogues")]
    [SerializeField] private GameObject dialogueBubble;
    [SerializeField] private float dialogueBubbleCooldown;
    
    [Header("Shocked")]
    [SerializeField] private GameObject shockedBubble;

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
        
        bubble.GetComponent<DialogueBubble>().InitBubble(ai);
        bubble.GetComponent<DialogueBubble>().InitSpriteAndText(typeOfDialogue);

        cooldownTimer = dialogueBubbleCooldown;

        Destroy(bubble, bubbleDuration);
        
        return true;
    }

    public void SpawnShockedBubble(GameObject ai)
    {
        var targetScreenPos = Camera.main.WorldToScreenPoint(ai.transform.position);
        
        var bubble = Instantiate(shockedBubble, targetScreenPos, Quaternion.identity, canvas.transform);
        
        bubble.GetComponent<ShockedBubble>().InitBubble(ai);
        
        Destroy(bubble, bubbleDuration);
    }
}
