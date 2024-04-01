using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    public async Task InitAsync()
    {
        //UGS 서비스 인증파트가 들어갈 예정입니다.
    }

    public void GotoMenuScene()
    {
        SceneManager.LoadScene(SceneNames.MenuScene);
    }
    
}
