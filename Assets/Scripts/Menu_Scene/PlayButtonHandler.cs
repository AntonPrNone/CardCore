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
            Debug.LogError("PlayButtonHandler: ��� ����� �� �������!");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"PlayButtonHandler: ����� '{sceneName}' �� ��������� � Build Settings!");
        }
    }
}
