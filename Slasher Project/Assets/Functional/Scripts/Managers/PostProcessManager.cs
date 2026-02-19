using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PostProcessManager : MonoBehaviour
{
    public static PostProcessManager Instance;

    [SerializeField] private Volume volume;
    [SerializeField] private float onKillDuration;
    private float onKillTimer;
    private bool isInKillMode;

    [SerializeField] private Canvas canva;
    [SerializeField] private GameObject bloodSplatters;
    [SerializeField] private Sprite[] randomSprites;

    [SerializeField] private bool bloodSplatterOn;
    
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
        if (!isInKillMode) return;
        onKillTimer -= Time.deltaTime;

        if (onKillTimer <= 0)
        {
            volume.weight = 0;
            isInKillMode = false;
        }
    }
    
    public void OnKill()
    {
        volume.weight = .5f;
        isInKillMode = true;
        onKillTimer = onKillDuration;

        if (!bloodSplatterOn)return;
        
        var blood = Instantiate(bloodSplatters, canva.transform.position, Quaternion.identity, canva.transform);
        bloodSplatters.GetComponent<Image>().sprite = randomSprites[Random.Range(0, randomSprites.Length)];
        Destroy(blood, 2f);
    }
}
