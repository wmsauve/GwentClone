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
    public void HandleSpell(GwentCard _playedCard, List<C_PlayerGamePlayLogic> _players, ulong _whosCard)
    {
        var _effect = _playedCard.cardEffects;
        foreach(EnumCardEffects effect in _effect)
        {
            switch (effect)
            {
                case EnumCardEffects.Scorch: Scorch(_players, _playedCard.scorchTarget, _playedCard.scorchAmount, _whosCard); break;
                case EnumCardEffects.Spy: Spy(_players.Find(x => x.ReturnID() == _whosCard)); break;
                case EnumCardEffects.Medic: Medic(_players.Find(x => x.ReturnID() == _whosCard)); break;
                case EnumCardEffects.Muster: Muster(_players, _whosCard, _playedCard); break;
                case EnumCardEffects.MoraleBoost: 
                case EnumCardEffects.CommandersHorn: PowerAdjustRelated(_players, _playedCard); break;
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
    public void HandleSpell(List<EnumCardEffects> _effect, S_GamePlayLogicManager.CardToClient[] _interact, EnumUnitPlacement _place, C_PlayerGamePlayLogic _player, int _cardSlot, GwentCard _played)
    {
        foreach(EnumCardEffects effect in _effect)
        {
            switch (effect)
            {
                case EnumCardEffects.Decoy: Decoy(_player, _cardSlot, _interact, _place, _played); break;
            }
        }
    }

    private void Scorch(List<C_PlayerGamePlayLogic> _players, EnumUnitPlacement _scorchTarget, int _scorchAmount, ulong _whosCard)
    {
        if (_gameManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You need Game manager reference here.");
            return;
        }

        //Scorch card
        if (_scorchTarget == EnumUnitPlacement.AnyPlayer)
        {
            //Determine highest score.
            int _highestScore = 0;
            foreach (C_PlayerGamePlayLogic _player in _players)
            {
                List<GwentCard> _highestCards = _player.CardsInPlay.HighestPowerCard;
                if (_highestCards.Count == 0) continue;

                if (_highestCards[0].cardPower > _highestScore) _highestScore = _highestCards[0].cardPower;
            }

            //Destroy cards of highest power.
            if (_highestScore == 0) return;

            foreach (C_PlayerGamePlayLogic _player in _players)
            {
                List<GwentCard> _highestCards = _player.CardsInPlay.HighestPowerCard;
                if (_highestCards.Count == 0) continue;

                if (_highestCards[0].cardPower == _highestScore) _player.PlaceCardInGraveyardScorch();
            }

            _gameManager.DestroyCardsFromEffectClientRpc(_highestScore, _scorchTarget);

        }

        //Targetted Scorch
        else if (
            _scorchTarget == EnumUnitPlacement.Frontline ||
            _scorchTarget == EnumUnitPlacement.Ranged ||
            _scorchTarget == EnumUnitPlacement.Siege)
        {
            C_PlayerGamePlayLogic _other = _players.Find(x => x.ReturnID() != _whosCard);
            C_PlayerGamePlayLogic _player = _players.Find(x => x.ReturnID() == _whosCard);

            if(_other == null || _player == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Failed to find other player for scorching their cards.");
                return;
            }

            S_GameZones.GameZone _zone = null;

            switch (_scorchTarget)
            {
                case EnumUnitPlacement.Frontline: _zone = _other.CardsInPlay.CardsInFront; break;
                case EnumUnitPlacement.Ranged: _zone = _other.CardsInPlay.CardsInRanged; break;
                case EnumUnitPlacement.Siege: _zone = _other.CardsInPlay.CardsInSiege; break;
            }

            if (_zone == null)
            {
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Failed to zone to scorch opponent cards.");
                return;
            }

            //Destroy cards.
            if (_zone.TotalPower >= _scorchAmount)
            {
                _other.PlaceCardInGraveyardScorch(_scorchTarget, _zone);
                _gameManager.DestroyCardsFromEffectClientRpc(_zone.FlaggedPowerForDestroy, _scorchTarget, false, _player.ClientRpcParams);
                _gameManager.DestroyCardsFromEffectClientRpc(_zone.FlaggedPowerForDestroy, _scorchTarget, true, _other.ClientRpcParams);
            }
        }
    }

    private void Spy(C_PlayerGamePlayLogic _player)
    {
        if (_gameManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You need Game manager reference here.");
            return;
        }

        //Spies draw 2, but think about making this variable in the future.
        int _numToDraw = 2;
        List<GwentCard> _drawnCards = _player.DrawCardFromDeck(_numToDraw);
        S_GamePlayLogicManager.CardToClient[] cardNames = new S_GamePlayLogicManager.CardToClient[_drawnCards.Count];
        for(int i = 0; i < _drawnCards.Count; i++)
        {
            S_GamePlayLogicManager.CardToClient _cardToClient = new S_GamePlayLogicManager.CardToClient();
            _cardToClient._card = _drawnCards[i].id;
            _cardToClient._unique = _drawnCards[i].UniqueGuid;
            cardNames[i] = _cardToClient;
        }
        //var _json = JsonUtility.ToJson(new S_GamePlayLogicManager.CardNames(cardNames));
        var _json = GeneralPurposeFunctions.ConvertArrayToJson(cardNames);
        _gameManager.PlaceCardInHandClientRpc(_json, _player.ClientRpcParams);
    }

    private void Decoy(C_PlayerGamePlayLogic _play, int _cardSlot, S_GamePlayLogicManager.CardToClient[] _interact, EnumUnitPlacement _place, GwentCard _played)
    {
        List<GwentCard> _zone = null;
        //decoy only affects one card
        var _cardsInPlay = _play.CardsInPlay;
        switch (_place)
        {
            case EnumUnitPlacement.Frontline: _zone = _cardsInPlay.CardsInFront.Cards; break;
            case EnumUnitPlacement.Ranged: _zone = _cardsInPlay.CardsInRanged.Cards; break;
            case EnumUnitPlacement.Siege: _zone = _cardsInPlay.CardsInSiege.Cards; break;
        }
        var _loc = _zone.FindIndex(x => x.UniqueGuid == _interact[0]._unique); //outline object exists at placement = 0
        GwentCard _toHand = _zone[_loc];
        _zone.RemoveAt(_loc);
        _zone.Insert(_loc, _played);

        _play.CardsInHand.RemoveAt(_cardSlot);
        _play.CardsInHand.Insert(_cardSlot, _toHand);
    }

    private void Medic(C_PlayerGamePlayLogic _player)
    {
        if (_gameManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You need Game manager reference here.");
            return;
        }

        _gameManager.OpenGraveyardUIClientRpc(_player.ClientRpcParams);
    }

    private void Muster(List<C_PlayerGamePlayLogic> _players, ulong _whos, GwentCard _card)
    {
        if (_gameManager == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "You need Game manager reference here.");
            return;
        }

        C_PlayerGamePlayLogic _play = _players.Find(x => x.ReturnID() == _whos);

        string _musterTag = _card.musterTag;
        List<GwentCard> _cardsToPlay = new List<GwentCard>();

        Debug.LogWarning(_play.MyInfo.Deck.Cards.Count + " cards in deck.");

        List<GwentCard> _handCards = _play.RemoveMusterCardsFromHand(_musterTag, _card);

        _cardsToPlay.AddRange(_play.RemoveMusterCardsFromDeck(_musterTag));
        _cardsToPlay.AddRange(_handCards);

        Debug.LogWarning(_play.MyInfo.Deck.Cards.Count + " cards in deck after muster logic on server.");

        S_GamePlayLogicManager.CardToClient[] _ids = _play.ReturnCardIds(EnumCardListType.Hand, _cardsToPlay);
        //var _cardString = JsonUtility.ToJson(new S_GamePlayLogicManager.CardNames(_ids));
        var _json = GeneralPurposeFunctions.ConvertArrayToJson(_ids);

        foreach (C_PlayerGamePlayLogic player in _players)
        {
            _gameManager.PlayMultipleCardsClientRpc(_json, player.ClientRpcParams);
        }
    }

    private void PowerAdjustRelated(List<C_PlayerGamePlayLogic> _players, GwentCard _card)
    {
        foreach(C_PlayerGamePlayLogic player in _players)
        {
            //player.
        }
    }
}
