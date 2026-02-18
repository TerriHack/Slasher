using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private bool phaseZeroEnded;

    public Action startFleeingPhase;

    private GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        player = GameObject.FindGameObjectWithTag("Player"); //DEBUG
    }
    
    public void FirstBlood()
    {
        if (phaseZeroEnded) return;
        
        Debug.Log("FIIIIIIRST BLOOOOOOOOD");
        
        phaseZeroEnded = true;
        startFleeingPhase?.Invoke();
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public GameObject GetPlayer()
    {
        return player;
    }
}
