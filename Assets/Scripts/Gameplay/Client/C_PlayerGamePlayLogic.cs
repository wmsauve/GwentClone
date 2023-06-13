using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class C_PlayerGamePlayLogic : NetworkBehaviour
{
    private S_GamePlayLogicManager _logicManager = null;

    private NetworkVariable<bool> _turnActive = new NetworkVariable<bool>(false);
    public bool TurnActive { get { return _turnActive.Value; } set { _turnActive.Value = value; } }
    private NetworkVariable<int> _mulligans = new NetworkVariable<int>(GlobalConstantValues.GAME_MULLIGANSAMOUNT);
    public int Mulligans { get { return _mulligans.Value; } }

    private GwentPlayer _myInfo = null;
    public GwentPlayer MyInfo { get { return _myInfo; } }

    private List<Card> _cardsInHand = new List<Card>();
    public List<Card> CardsInHand { get { return _cardsInHand; } }
    private List<Card> _cardsInGraveyard = new List<Card>();
    public List<Card> CardsInGraveyard { get { return _cardsInGraveyard; } }

    public S_GameZones _cardsInPlay = new S_GameZones();

    //Store card here to prevent mulliganing cards into the exact same card that you mulliganed away.
    private List<Card> _mulliganStorage = new List<Card>();

    private ClientRpcParams _params;
    public ClientRpcParams ClientRpcParams { get { return _params; } }

    private int _lives = GlobalConstantValues.GAME_INITIALLIVES;

    private int _initialHandSize = GlobalConstantValues.GAME_INITIALHANDSIZE;

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

    private void Update()
    {
        if (IsClient && IsOwner)
        {
        }
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

    #region Deal With Cards Related

    public void SuccessfullyPlayCards(int cardSlot, EnumUnitPlacement cardPlace)
    {
        Card playedCard = _cardsInHand[cardSlot];
        _cardsInHand.RemoveAt(cardSlot);

        switch (cardPlace)
        {
            case EnumUnitPlacement.Frontline:
                _cardsInPlay.CardsInFront.Add(playedCard);
                break;
            case EnumUnitPlacement.Ranged:
                _cardsInPlay.CardsInRanged.Add(playedCard);
                break;
            case EnumUnitPlacement.Siege:
                _cardsInPlay.CardsInSiege.Add(playedCard);
                break;
        }
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
    public string[] ReturnCardIds()
    {
        List<string> toClient = new List<string>();
        for (int i = 0; i < _initialHandSize; i++) toClient.Add(_cardsInHand[i].id);
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
