using UnityEngine;

public class MenuPlayButton : MonoBehaviour
{
    [SerializeField] private string LevelToLoadName;

    public void StartLevel()
    {
        SceneLoader.LoadScene(LevelToLoadName);
    }
}
