using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class S_SpellsManager : NetworkBehaviour
{
    public S_GamePlayLogicManager _gameManager = null;

    /// <summary>
    /// Used for: Scorch, Spy
    /// </summary>
    /// <param name="_effect"></param>
    /// <param name="_players"></param>
    /// <param name="_whosCard"></param>
    public void HandleSpell(List<EnumCardEffects> _effect, List<C_PlayerGamePlayLogic> _players, ulong _whosCard)
    {
        foreach(EnumCardEffects effect in _effect)
        {
            switch (effect)
            {
                case EnumCardEffects.Scorch: Scorch(_players, _whosCard); break;
                case EnumCardEffects.Spy: Spy(_players.Find(x => x.ReturnID() == _whosCard)); break;
            }
        }

    }

    /// <summary>
    /// Used for: Decoy
    /// </summary>
    /// <param name="_effect"></param>
    /// <param name="_interact"></param>
    /// <param name="_place"></param>
    /// <param name="_player"></param>
    /// <param name="_cardSlot"></param>
    /// <param name="_played"></param>
    public void HandleSpell(List<EnumCardEffects> _effect, List<S_GamePlayLogicManager.InteractCardsOnServer> _interact, EnumUnitPlacement _place, C_PlayerGamePlayLogic _player, int _cardSlot, Card _played)
    {
        foreach(EnumCardEffects effect in _effect)
        {
            switch (effect)
            {
                case EnumCardEffects.Decoy: Decoy(_player, _cardSlot, _interact, _place, _played); break;
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

    private void Spy(C_PlayerGamePlayLogic _player)
    {
        //Spies draw 2, but think about making this variable in the future.
        int _numToDraw = 2;
        List<Card> _drawnCards = _player.DrawCardFromDeck(_numToDraw);
        string[] cardNames = new string[_drawnCards.Count];
        for(int i = 0; i < _drawnCards.Count; i++) cardNames[i] = _drawnCards[i].id;
        var _json = JsonUtility.ToJson(new S_GamePlayLogicManager.CardNames(cardNames));

        Debug.LogWarning(_json + " yo guy?");

        _gameManager.PlaceCardInHandClientRpc(_json, _player.ClientRpcParams);
    }

    private void Decoy(C_PlayerGamePlayLogic _play, int _cardSlot, List<S_GamePlayLogicManager.InteractCardsOnServer> _interact, EnumUnitPlacement _place, Card _played)
    {
        List<Card> _zone = null;
        //decoy only affects one card
        switch (_place)
        {
            case EnumUnitPlacement.Frontline: _zone = _play.CardsInPlay.CardsInFront; break;
            case EnumUnitPlacement.Ranged: _zone = _play.CardsInPlay.CardsInRanged; break;
            case EnumUnitPlacement.Siege: _zone = _play.CardsInPlay.CardsInSiege; break;
        }
        var _loc = _interact[0]._placement - 1; //outline object exists at placement = 0
        Debug.LogWarning(_loc + " where is target card in zone.");
        _zone.RemoveAt(_loc);
        _zone.Insert(_loc, _played);

        _play.CardsInHand.RemoveAt(_cardSlot);
        _play.CardsInHand.Insert(_cardSlot, _interact[0]._card);
    }
}
