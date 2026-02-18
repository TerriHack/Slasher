using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] private Image bubbleImage;
    [SerializeField] private TextMeshProUGUI bubbleText;
    private GameObject target;

    public void InitBubble(Sprite sprite, string text, GameObject target)
    {
        //bubbleImage.sprite = sprite;
        bubbleText.text = text;
        
        this.target = target;
    }

    private void Update()
    {
        var targetScreenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        transform.position = targetScreenPos;
    }

}
