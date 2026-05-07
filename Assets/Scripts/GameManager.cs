using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] GameObject R_btn;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }


    public event Action OnResetBallPos;
    public void ResetBallPos()
    {
        OnResetBallPos?.Invoke();
        R_btn.SetActive(false);
    }

    public void ShowRestartBtn()
    {
        R_btn.SetActive(true);
    }

}
