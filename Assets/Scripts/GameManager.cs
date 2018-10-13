using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one instance of GameManager is trying to active");
            return;
        }

        instance = this;
    }

    #endregion

    [SerializeField] private PauseMenuScript menu;
    [SerializeField] private CameraRotation playerCamera;

    public void FixCameraTarget()
    {
        playerCamera.FixCamera();
    }

    public void EndLevelLost()
    {
        menu.die.StartFade();
    }


}
