using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
