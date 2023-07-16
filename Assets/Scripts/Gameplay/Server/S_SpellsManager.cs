using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class S_SpellsManager : NetworkBehaviour
{
    public S_GamePlayLogicManager _gameManager = null;

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
        //Todo: change this to use unity placement to determine where units are destroyed. Scorch is global, scorch battlecry is per placement.

        //Determine highest score.
        int _highestScore = 0;
        foreach(C_PlayerGamePlayLogic _player in _players)
        {
            List<Card> _highestCards = _player.CardsInPlay.HighestPowerCard;
            if (_highestCards.Count == 0) continue;

            if (_highestCards[0].cardPower > _highestScore) _highestScore = _highestCards[0].cardPower;
        }

        //Destroy cards of highest power.
        if (_highestScore == 0) return;

        foreach(C_PlayerGamePlayLogic _player in _players)
        {
            List<Card> _highestCards = _player.CardsInPlay.HighestPowerCard;
            if (_highestCards.Count == 0) continue;

            if (_highestCards[0].cardPower == _highestScore) _player.PlaceCardInGraveyardScorch(); 
        }

        if (_gameManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You need Game manager reference here.");
            return;
        }

        _gameManager.DestroyCardsFromEffectClientRpc(_highestScore);
    }
}
