using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class C_PlayerGamePlayLogic : NetworkBehaviour
{
    private S_GamePlayLogicManager _logicManager = null;

    private NetworkVariable<bool> _turnActive = new NetworkVariable<bool>(false);
    public bool TurnActive { get { return _turnActive.Value; } set { _turnActive.Value = value; } }

    private GwentPlayer _myInfo = null;
    public GwentPlayer MyInfo { get { return _myInfo; } }

    private List<Card> _cardsInHand = new List<Card>();
    public List<Card> CardsInHand { get { return _cardsInHand; } }
    private List<Card> _cardsInGraveyard = new List<Card>();
    public List<Card> CardsInGraveyard { get { return _cardsInGraveyard; } }

    //Store card here to prevent mulliganing cards into the exact same card that you mulliganed away.
    private List<Card> _mulliganStorage = new List<Card>();

    private ClientRpcParams _params;
    public ClientRpcParams ClientRpcParams { get { return _params; } }

    private int _initialHandSize = GlobalConstantValues.GAME_INITIALHANDSIZE;
    private int _mulligans = GlobalConstantValues.GAME_MULLIGANSAMOUNT;

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

    public string MulliganCard(string mulliganed)
    {
        if (_mulligans > 0)
        {
            _mulligans--;

            //Get new card.
            int which = Random.Range(0, _myInfo.Deck.Cards.Count);
            Card newHandCard = _myInfo.Deck.Cards[which];
            _myInfo.Deck.RemoveCard(newHandCard);

            Debug.LogWarning(newHandCard.id);

            //Place new card into mulliganed slot.
            int cardIndex = _cardsInHand.FindIndex((card) => card.id == mulliganed);
            _cardsInHand.RemoveAt(cardIndex);
            _cardsInHand.Insert(cardIndex, newHandCard);

            return newHandCard.id;
        }

        return string.Empty;
        
    }
}
