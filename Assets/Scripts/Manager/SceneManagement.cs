using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagement : Singleton<SceneManagement>
{
    public string SceneName { get; private set; }
    public string SceneTransitionName { get; private set; }

    public void SetTransitionName(string sceneTransitionName) 
    {
        this.SceneTransitionName = sceneTransitionName;
    }
}

