using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class C_PlayerGamePlayLogic : NetworkBehaviour
{
    public struct StoreAdditionalStepCards
    {
        public GwentCard CardData;
        public EnumUnitPlacement CardPlace;
        public int CardSlot;
    } 

    private NetworkVariable<bool> _turnActive = new NetworkVariable<bool>(false);
    public bool TurnActive { 
        get { return _turnActive.Value; } 
        set 
        {
            _tempAdditionalStepCards.Clear();
            _turnActive.Value = value; 
        } 
    }
    private NetworkVariable<int> _mulligans = new NetworkVariable<int>(GlobalConstantValues.GAME_MULLIGANSAMOUNT);
    public int Mulligans { get { return _mulligans.Value; } }

    private GwentPlayer _myInfo = null;
    public GwentPlayer MyInfo { get { return _myInfo; } }

    private List<GwentCard> _cardsInHand = new List<GwentCard>();
    public List<GwentCard> CardsInHand { get { return _cardsInHand; } }
    private List<GwentCard> _cardsInGraveyard = new List<GwentCard>();
    public List<GwentCard> CardsInGraveyard { get { return _cardsInGraveyard; } }

    private S_GameZones _cardsInPlay = new S_GameZones();
    public S_GameZones CardsInPlay { get { return _cardsInPlay; } }

    //Store card here to prevent mulliganing cards into the exact same card that you mulliganed away.
    private List<Card> _mulliganStorage = new List<Card>();

    private ClientRpcParams _params;
    public ClientRpcParams ClientRpcParams { get { return _params; } }

    private int _lives = GlobalConstantValues.GAME_INITIALLIVES;
    public int Lives { get { return _lives; } }

    private int _initialHandSize = GlobalConstantValues.GAME_INITIALHANDSIZE;

    private List<StoreAdditionalStepCards> _tempAdditionalStepCards = new List<StoreAdditionalStepCards>();
    public List<StoreAdditionalStepCards> MultiStepCards { get { return _tempAdditionalStepCards; } }

    private List<StoreAdditionalStepCards> _storeGraveyardUntilEndOfTurn = new List<StoreAdditionalStepCards>();

    public void InitializePlayerLogic(GwentPlayer player)
    {
        _myInfo = player;
        _params = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { _myInfo.ID }
            }
        };
    }
    public S_GamePlayLogicManager.CardToClient[] CreateInitialHand()
    {
        List<S_GamePlayLogicManager.CardToClient> toClient = new List<S_GamePlayLogicManager.CardToClient>();

        for(int i = 0; i < _initialHandSize; i++)
        {
            int which = Random.Range(0, _myInfo.Deck.Cards.Count);
            Card inHand = _myInfo.Deck.Cards[which];
            _myInfo.Deck.RemoveCard(inHand);
            GwentCard _newCard = new GwentCard(inHand);

            S_GamePlayLogicManager.CardToClient _clientCard = new S_GamePlayLogicManager.CardToClient(_newCard.id, _newCard.UniqueGuid);

            toClient.Add(_clientCard);
            _cardsInHand.Add(_newCard);
        }

        return toClient.ToArray();
    }

    #region Graveyard Related
    public void StoreReferenceToPlayingMultiStepCard(GwentCard _card, EnumUnitPlacement _cardPlace, int _cardSlot)
    {
        StoreAdditionalStepCards _temp;
        _temp.CardData = _card;
        _temp.CardPlace = _cardPlace;
        _temp.CardSlot = _cardSlot;

        _tempAdditionalStepCards.Add(_temp);
    }

    public void PlaceCardInGraveyardScorch(EnumUnitPlacement _placement = EnumUnitPlacement.AnyPlayer, S_GameZones.GameZone _zone = null)
    {
        if (_zone != null)
        {
            foreach (GwentCard _card in _zone.HighestPowerCards) _cardsInGraveyard.Add(_card);
            _cardsInPlay.DestroyCardsOfPowerInPlay(_placement, _zone.HighestPowerCard);
        }
        else
        {
            foreach (GwentCard _card in _cardsInPlay.HighestPowerCard) _cardsInGraveyard.Add(_card);
            _cardsInPlay.DestroyCardsOfPowerInPlay(_placement);
        }
    }

    public void EndOfTurnGraveyardCards()
    {
        foreach(GwentCard card in _cardsInPlay.CardsInFront.Cards)
        {
            _cardsInGraveyard.Add(card);
        }
        _cardsInPlay.CardsInFront.Cards.Clear();

        foreach (GwentCard card in _cardsInPlay.CardsInRanged.Cards)
        {
            _cardsInGraveyard.Add(card);
        }
        _cardsInPlay.CardsInRanged.Cards.Clear();

        foreach (GwentCard card in _cardsInPlay.CardsInSiege.Cards)
        {
            _cardsInGraveyard.Add(card);
        }
        _cardsInPlay.CardsInSiege.Cards.Clear();
        _cardsInPlay.HighestPowerCard.Clear();
    }

    public void RemoveCardFromGraveyard(int _slot)
    {
        StoreAdditionalStepCards _card = new StoreAdditionalStepCards();
        _card.CardData = _cardsInGraveyard[_slot];
        _card.CardSlot = _slot;
        _storeGraveyardUntilEndOfTurn.Add(_card);
        _cardsInGraveyard.RemoveAt(_slot);
    }

    public void RunCheckUnresolvedCards()
    {
        if (_storeGraveyardUntilEndOfTurn.Count <= 0) return;

        foreach(StoreAdditionalStepCards _card in _storeGraveyardUntilEndOfTurn)
        {
            _cardsInGraveyard.Add(_card.CardData);
        }

        _storeGraveyardUntilEndOfTurn.Clear();
    }
    #endregion Graveyard Related

    #region Game Related
    public void DecrementLives()
    {
        _lives--;
    }
    #endregion Game Related

    #region Deal With Cards Related

    public void RemoveCardFromHandServer(int cardSlot)
    {
        _cardsInHand.RemoveAt(cardSlot);
    }

    public void PlaceCardInPlay(GwentCard playedCard, EnumUnitPlacement cardPlace)
    {
        switch (cardPlace)
        {
            case EnumUnitPlacement.Frontline:
                _cardsInPlay.CardsInFront.AddCardToZone(playedCard);
                break;
            case EnumUnitPlacement.Ranged:
                _cardsInPlay.CardsInRanged.AddCardToZone(playedCard);
                break;
            case EnumUnitPlacement.Siege:
                _cardsInPlay.CardsInSiege.AddCardToZone(playedCard);
                break;
        }

        _cardsInPlay.CheckForNewHighestCard();
    }

    public List<GwentCard> RemoveMusterCardsFromHand(string _tag, GwentCard _played)
    {
        List<GwentCard> _playedCard = new List<GwentCard>();
        List<GwentCard> _hand = _cardsInHand;

        for (int i = _hand.Count - 1; i >= 0; i--)
        {
            //We are not mustering the muster card that was played.
            //if (_hand[i].UniqueMusterID == _played.UniqueMusterID)
            //{
            //    Debug.LogWarning($" is this a thing? {i} _hand id {_hand[i].UniqueMusterID} _played id {_played.UniqueMusterID}");
            //    continue;
            //}

            if (_hand[i].cardEffects.Contains(EnumCardEffects.Muster) && _hand[i].musterTag == _tag)
            {
                _playedCard.Add(_hand[i]);
                EnumUnitPlacement _placement = _hand[i].unitPlacement;
                PlaceCardInPlay(_hand[i], _placement);
                RemoveCardFromHandServer(i);
            }
        }

        return _playedCard;
    }

    public List<GwentCard> RemoveMusterCardsFromDeck(string _tag)
    {
        List<GwentCard> _drawnCards = new List<GwentCard>();

        List<Card> _deck = _myInfo.Deck.Cards;

        for (int i = _deck.Count - 1; i >= 0; i--)
        {
            if(_deck[i].cardEffects.Contains(EnumCardEffects.Muster) && _deck[i].musterTag == _tag)
            {
                GwentCard _musterCard = new GwentCard(_deck[i]);
                EnumUnitPlacement _placement = _musterCard.unitPlacement;
                PlaceCardInPlay(_musterCard, _placement);
                _deck.RemoveAt(i);
                _drawnCards.Add(_musterCard);
            }
        }

        return _drawnCards;
    }

    public List<GwentCard> DrawCardFromDeck(int _num)
    {
        List<GwentCard> _drawnCards = new List<GwentCard>();
        for(int i = 0; i < _num; i++)
        {
            Card _drawnCard = _myInfo.Deck.Cards[0];
            GwentCard _newGwentCard = new GwentCard(_drawnCard);
            _myInfo.Deck.Cards.Remove(_drawnCard);

            _cardsInHand.Add(_newGwentCard);
            _drawnCards.Add(_newGwentCard);
        }

        return _drawnCards;
    }
    #endregion Deal With Cards Related

    #region Mulligan Related
    public string MulliganCard(string mulliganed)
    {
        if (_mulligans.Value > 0)
        {
            _mulligans.Value--;

            //Get new card.
            int which = Random.Range(0, _myInfo.Deck.Cards.Count);
            Card newHandCard = _myInfo.Deck.Cards[which];
            _myInfo.Deck.RemoveCard(newHandCard);

            Debug.LogWarning(newHandCard.id);

            //Place new card into mulliganed slot.
            int cardIndex = _cardsInHand.FindIndex((card) => card.id == mulliganed);
            Card oldCard = _cardsInHand[cardIndex].DataRef;
            _mulliganStorage.Add(oldCard);
            _cardsInHand.RemoveAt(cardIndex);
            GwentCard _gwentCard = new GwentCard(newHandCard);
            _cardsInHand.Insert(cardIndex, _gwentCard);

            return newHandCard.id;
        }

        return string.Empty;

    }

    public void EndMulliganPhase()
    {
        if (_mulliganStorage.Count < 1) return;

        _myInfo.Deck.Cards.AddRange(_mulliganStorage);
    }

    #endregion Mulligan Related

    #region Utility Related
    public S_GamePlayLogicManager.CardToClient[] ReturnCardIds(EnumCardListType which, List<GwentCard> _cards = null)
    {
        List<GwentCard> _list = new List<GwentCard>();

        if(_cards == null)
        {
            switch (which)
            {
                case EnumCardListType.Hand: _list = _cardsInHand; break;
                case EnumCardListType.Graveyard: _list = _cardsInGraveyard; break;
                default: break;
            }
        }
        else _list = _cards;


        List<S_GamePlayLogicManager.CardToClient> toClient = new List<S_GamePlayLogicManager.CardToClient>();
        for (int i = 0; i < _list.Count; i++)
        {
            S_GamePlayLogicManager.CardToClient _toClient = new S_GamePlayLogicManager.CardToClient();
            _toClient._card = _list[i].id;
            _toClient._unique = _list[i].UniqueGuid;
            toClient.Add(_toClient);
        }
        return toClient.ToArray();
    }

    public bool ReturnOwnerStatus()
    {
        var netObj = gameObject.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "This should be on a network object.");
            return false;
        }

        if (netObj.IsOwner)
        {
            return true;
        }

        return false;
    }

    public ulong ReturnID()
    {
        var netObj = gameObject.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.MissingComponent, "This should be on a network object.");
            return 0;
        }

        return netObj.OwnerClientId;
    }

    #endregion Utility Related

}
