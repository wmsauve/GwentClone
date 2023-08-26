using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class C_PlayerGamePlayLogic : NetworkBehaviour
{
    private struct StoreAdditionalStepCards
    {
        public Card CardData;
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

    private List<Card> _cardsInHand = new List<Card>();
    public List<Card> CardsInHand { get { return _cardsInHand; } }
    private List<Card> _cardsInGraveyard = new List<Card>();
    public List<Card> CardsInGraveyard { get { return _cardsInGraveyard; } }

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
    public string[] CreateInitialHand()
    {
        List<string> toClient = new List<string>();

        for(int i = 0; i < _initialHandSize; i++)
        {
            int which = Random.Range(0, _myInfo.Deck.Cards.Count);
            Card inHand = _myInfo.Deck.Cards[which];
            _myInfo.Deck.RemoveCard(inHand);
            toClient.Add(inHand.id);
            _cardsInHand.Add(inHand);
        }

        return toClient.ToArray();
    }

    #region Graveyard Related
    public void StoreReferenceToPlayingMultiStepCard(Card _card, EnumUnitPlacement _cardPlace, int _cardSlot)
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
            foreach (Card _card in _zone.HighestPowerCards) _cardsInGraveyard.Add(_card);
            _cardsInPlay.DestroyCardsOfPowerInPlay(_placement, _zone.HighestPowerCard);
        }
        else
        {
            foreach (Card _card in _cardsInPlay.HighestPowerCard) _cardsInGraveyard.Add(_card);
            _cardsInPlay.DestroyCardsOfPowerInPlay(_placement);
        }
    }

    public void EndOfTurnGraveyardCards()
    {
        foreach(Card card in _cardsInPlay.CardsInFront.Cards)
        {
            _cardsInGraveyard.Add(card);
        }
        _cardsInPlay.CardsInFront.Cards.Clear();

        foreach (Card card in _cardsInPlay.CardsInRanged.Cards)
        {
            _cardsInGraveyard.Add(card);
        }
        _cardsInPlay.CardsInRanged.Cards.Clear();

        foreach (Card card in _cardsInPlay.CardsInSiege.Cards)
        {
            _cardsInGraveyard.Add(card);
        }
        _cardsInPlay.CardsInSiege.Cards.Clear();
    }
    #endregion Graveyard Related

    #region Game Related
    public void DecrementLives()
    {
        _lives--;
    }
    #endregion Game Related

    #region Deal With Cards Related

    public void RemoveCardFromHandServer(int cardSlot, EnumUnitPlacement cardPlace)
    {
        _cardsInHand.RemoveAt(cardSlot);
    }

    public void PlaceCardInPlay(Card playedCard, EnumUnitPlacement cardPlace)
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

    public List<Card> DrawCardFromDeck(int _num)
    {
        List<Card> _drawnCards = new List<Card>();
        for(int i = 0; i < _num; i++)
        {
            Card _drawnCard = _myInfo.Deck.Cards[0];
            _myInfo.Deck.Cards.Remove(_drawnCard);

            _cardsInHand.Add(_drawnCard);
            _drawnCards.Add(_drawnCard);
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
            Card oldCard = _cardsInHand[cardIndex];
            _mulliganStorage.Add(oldCard);
            _cardsInHand.RemoveAt(cardIndex);
            _cardsInHand.Insert(cardIndex, newHandCard);

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
    public string[] ReturnCardIds(EnumCardListType which)
    {
        List<Card> _list = new List<Card>();

        switch (which)
        {
            case EnumCardListType.Hand: _list = _cardsInHand; break;
            case EnumCardListType.Graveyard: _list = _cardsInGraveyard; break;
            default: break;
        }

        List<string> toClient = new List<string>();
        for (int i = 0; i < _list.Count; i++) toClient.Add(_list[i].id);
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
