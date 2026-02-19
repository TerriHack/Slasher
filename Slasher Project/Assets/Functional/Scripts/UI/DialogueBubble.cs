using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : Bubble
{
    [SerializeField] private Image bubbleImage;
    [SerializeField] private TextMeshProUGUI bubbleText;
    
    [SerializeField] private List<Sprite> bubbleSprites;
    [SerializeField] private List<string> phaseZeroDialogues;
    [SerializeField] private List<string> fleeDialogues;
    [SerializeField] private List<string> attackDialogues;
    [SerializeField] private List<string> isHeGoneDialogues;

    public void InitSpriteAndText(int type)
    {
        int r = 0;
        string text ="";
        
        switch (type)
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
        
        bubbleImage.sprite = bubbleSprites[type];
        bubbleText.text = text;
    }

}
