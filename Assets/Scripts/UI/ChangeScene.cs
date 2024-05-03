using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("Battle");
    }

    public void quitGame(){
        Application.Quit();
    }
}