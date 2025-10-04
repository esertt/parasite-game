using UnityEngine;
using UnityEngine.SceneManagement;
public class mainMenu : MonoBehaviour
{
    public void playGame(){
        SceneManager.Loadscene(SceneManager.GetActiveScene().buildindex + 1);

    } 
    public void quitGame(){
        Application.Quit();
    }
    
}
