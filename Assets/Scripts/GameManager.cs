using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public enum SceneName
    {
        MainScene,
        TitleScene
    }

    private bool created = false;

    // Start is called before the first frame update
    void Start()
    {
        if (created == false)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
