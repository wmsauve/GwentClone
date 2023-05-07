using UnityEngine;
using Unity.Netcode;

public class C_PlayerGamePlayLogic : NetworkBehaviour
{
    private S_GamePlayLogicManager _logicManager = null;

    private NetworkVariable<bool> _turnActive = new NetworkVariable<bool>(false);
    public bool TurnActive { get { return _turnActive.Value; } set { _turnActive.Value = value; } }

    private GwentPlayer _myInfo = null;

    private int _mulligans = 2;

    public void InitializePlayerLogic(GwentPlayer player)
    {
        _myInfo = player;
    }

    private void Update()
    {
        if (IsClient && IsOwner)
        {

        }
    }

    public void MulliganPhase()
    {

    }



}
