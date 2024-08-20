using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public void load(string name)
    {
        SceneLoader.LoadScene(name);
    }
}
