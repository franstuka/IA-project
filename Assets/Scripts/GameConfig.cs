using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour {

    public int GraphicsLevel = 5;
    public float SoundLevel = 0.8f;

    //.....

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
