using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class MainMenu : MonoBehaviour
{
  
    [SerializeField] private string gameSceneName = "GameWorld";

    private Button playButton;
    private Button quitButton;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        playButton = root.Q<Button>("start-game-button");
        quitButton = root.Q<Button>("quit-button");

        playButton.clicked += OnPlayClicked;
        quitButton.clicked += OnQuitClicked;
    }

    private void OnDisable()
    {
        if (playButton != null) playButton.clicked -= OnPlayClicked;
        if (quitButton != null) quitButton.clicked -= OnQuitClicked;
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}


