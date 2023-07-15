using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class S_SpellsManager : NetworkBehaviour
{


    public void HandleSpell(List<EnumCardEffects> _effect, List<C_PlayerGamePlayLogic> _players, ulong _whosCard)
    {
        foreach(EnumCardEffects effect in _effect)
        {
            switch (effect)
            {
                case EnumCardEffects.Scorch: Scorch(_players, _whosCard); break;
            }
        }

    }

    private void Scorch(List<C_PlayerGamePlayLogic> _players, ulong _whosCard)
    {
        int _highestScore = 0;
        foreach(C_PlayerGamePlayLogic _player in _players)
        {
            _player.CardsInPlay;
        }
    }
}
