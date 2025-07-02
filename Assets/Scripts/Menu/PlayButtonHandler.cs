using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayButtonHandler : MonoBehaviour
{
    [SceneName]
    [SerializeField] private string sceneName;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnPlayClicked);
    }

    private void OnPlayClicked()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("PlayButtonHandler: »м€ сцены не указано!");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"PlayButtonHandler: —цена '{sceneName}' не добавлена в Build Settings!");
        }
    }
}
