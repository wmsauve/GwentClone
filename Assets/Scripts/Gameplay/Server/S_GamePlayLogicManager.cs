using Unity.Netcode;

public class S_GamePlayLogicManager : NetworkBehaviour
{


    private void OnEnable()
    {
        GlobalActions.OnGameStart += InitializePlayerLogic;
    }

    private void OnDisable()
    {
        GlobalActions.OnGameStart -= InitializePlayerLogic;

    }

    private void InitializePlayerLogic()
    {

    }
}
