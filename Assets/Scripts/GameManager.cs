using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }


    public event Action OnResetBallPos;
    public void ResetBallPos()
    {
        OnResetBallPos?.Invoke();
    }

}
