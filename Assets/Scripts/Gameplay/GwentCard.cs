using System.Collections.Generic;
using UnityEngine;

public class GwentCard 
{
    [Header("Identifier Related")]
    [Tooltip("Name of the card, but also used as identifier in code.")]
    public string id;
    [Tooltip("Sprite of card.")]
    public Sprite cardImage;

    [Header("Card Information Related")]
    public EnumUnitType unitType;
    public EnumCardType cardType;
    public EnumUnitPlacement unitPlacement;
    public EnumFactionType factionType;
    public List<EnumCardEffects> cardEffects;

    [Header("Interact With Mechanics Related")]
    public int cardPower;
    public int maxPerDeck;
    [Tooltip("Used for mustering. Usually the same as ID. Can be different if a specific muster mechanic is utilized.")]
    public string musterTag;
    [Tooltip("Specify scorch target since cards can either target their placement or any placement for scorching.")]
    public EnumUnitPlacement scorchTarget;
    [Tooltip("In case we want variable scorch thresholds.")]
    public int scorchAmount;

    private int basePower;
    private string uniqueGuid;
    public string UniqueGuid { get { return uniqueGuid; } }
    private Card _dataRef;
    public Card DataRef { get { return _dataRef; } }


    public GwentCard(Card card) 
    {
        uniqueGuid = System.Guid.NewGuid().ToString();
        InitializeCard(card);
    }

    public GwentCard(string oldGuid, Card card)
    {
        uniqueGuid = oldGuid;
        InitializeCard(card);
    }

    public void InitializeCard(Card card)
    {
        _dataRef = card;

        id = card.id;
        cardImage = card.cardImage;
        unitType = card.unitType;
        cardType = card.cardType;
        unitPlacement = card.unitPlacement;
        factionType = card.factionType;
        cardEffects = card.cardEffects;
        cardPower = card.cardPower;
        maxPerDeck = card.maxPerDeck;
        musterTag = card.musterTag;
        scorchTarget = card.scorchTarget;
        scorchAmount = card.scorchAmount;

        basePower = cardPower;
    }

    public void ResetToBasePower()
    {
        cardPower = basePower;
    }
}
