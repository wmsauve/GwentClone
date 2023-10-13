using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class S_GameZones
{
    public class GameZone
    {
        public List<Card> Cards = new List<Card>();
        public int TotalPower;

        public List<Card> HighestPowerCards = new List<Card>();
        public int HighestPowerCard;
        public int FlaggedPowerForDestroy;
        
        public void AddCardToZone(Card _newCard)
        {
            Cards.Add(_newCard);
            RunHighestCardCheck();
        }

        public void DestroyCardsOfPower(int power)
        {
            FlaggedPowerForDestroy = power;
            List<Card> savedCards = new List<Card>();
            foreach(Card _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero) || _card.cardPower != power) savedCards.Add(_card);
            }

            Cards = savedCards;
            RunHighestCardCheck();
        }

        public void ApplyWeather()
        {
            foreach(Card _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero)) continue;
                _card.cardPower = 1;
            }

            RunHighestCardCheck();
        }

        public void ApplyPowerFromEffect(EnumCardEffects _effect)
        {
            switch (_effect)
            {
                case EnumCardEffects.MoraleBoost:
                    int numOfChars = 0;
                    foreach(Card _card in Cards)
                    {
                        if (_card.cardEffects.Contains(EnumCardEffects.MoraleBoost)) numOfChars++;
                    }

                    foreach (Card _card in Cards)
                    {
                        if (_card.cardEffects.Contains(EnumCardEffects.Hero) && _card.cardEffects.Contains(EnumCardEffects.MoraleBoost)) continue;
                        _card.cardPower += numOfChars;
                    }
                    break;
                case EnumCardEffects.CommandersHorn:
                    foreach (Card _card in Cards)
                    {
                        if (_card.cardEffects.Contains(EnumCardEffects.Hero) && _card.cardEffects.Contains(EnumCardEffects.CommandersHorn)) continue;
                        _card.cardPower *= 2;
                    }
                    break;
                default:
                    Debug.LogWarning("You are not checking a card effect that alters card power.");
                    break;
            }

            RunHighestCardCheck();
        }

        public void ResetCards()
        {
            foreach (Card _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero)) continue;
                //_card.ResetToBasePower();
            }
        }

        private void RunHighestCardCheck()
        {
            TotalPower = 0;
            HighestPowerCard = 0;
            HighestPowerCards.Clear();

            if (Cards == null || Cards.Count == 0) return;

            foreach (Card _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero)) continue;

                TotalPower += _card.cardPower;

                if (_card.cardPower > HighestPowerCard)
                {
                    HighestPowerCard = _card.cardPower;
                    HighestPowerCards = new List<Card>();
                    HighestPowerCards.Add(_card);
                }

                else if (_card.cardPower == HighestPowerCard) HighestPowerCards.Add(_card);
            }
        }
    }

    public class StatChangeEffects
    {
        public bool FrontWeather = false;
        public bool RangedWeather = false;
        public bool SiegeWeather = false;
        public bool FrontHorns = false;
        public bool RangedHorns = false;
        public bool SiegeHorns = false;
        public bool FrontMoraleBoost = false;
        public bool RangedMoraleBoost = false;
        public bool SiegeMoraleBoost = false;
    }

    private GameZone _cardsInFront = new GameZone();
    private GameZone _cardsInRanged = new GameZone();
    private GameZone _cardsInSiege = new GameZone();

    private StatChangeEffects _effectsActive = new StatChangeEffects();

    public GameZone CardsInFront { get { return _cardsInFront; } }
    public GameZone CardsInRanged { get { return _cardsInRanged; } }
    public GameZone CardsInSiege { get { return _cardsInSiege; } }

    private List<Card> _highestPowerCards = new List<Card>();
    public List<Card> HighestPowerCard { get { return _highestPowerCards; } }

    private int _currentHighestPower;

    public void CheckForNewHighestCard()
    {
        _currentHighestPower = _cardsInFront.HighestPowerCard;
        if (_cardsInRanged.HighestPowerCard > _currentHighestPower) _currentHighestPower = _cardsInRanged.HighestPowerCard;
        if (_cardsInSiege.HighestPowerCard > _currentHighestPower) _currentHighestPower = _cardsInSiege.HighestPowerCard;

        //Store highest in a separate list
        _highestPowerCards.Clear();
        if (_cardsInFront.HighestPowerCard == _currentHighestPower
        && _cardsInFront.HighestPowerCards != null
        && _cardsInFront.HighestPowerCards.Count > 0) _highestPowerCards.AddRange(_cardsInFront.HighestPowerCards);
        if (_cardsInRanged.HighestPowerCard == _currentHighestPower
        && _cardsInRanged.HighestPowerCards != null
        && _cardsInRanged.HighestPowerCards.Count > 0) _highestPowerCards.AddRange(_cardsInRanged.HighestPowerCards);
        if (_cardsInSiege.HighestPowerCard == _currentHighestPower
        && _cardsInSiege.HighestPowerCards != null
        && _cardsInSiege.HighestPowerCards.Count > 0) _highestPowerCards.AddRange(_cardsInSiege.HighestPowerCards);
    }

    /// <summary>
    /// Use Highest card across all zones if not passing value for _power.
    /// </summary>
    /// <param name="_placement"></param>
    /// <param name="_power"></param>
    public void DestroyCardsOfPowerInPlay(EnumUnitPlacement _placement = EnumUnitPlacement.AnyPlayer, int _power = -1)
    {
        var _destroyThreshold = _power;
        if (_power == -1) _destroyThreshold = _currentHighestPower;
        if (_placement == EnumUnitPlacement.AnyPlayer || _placement == EnumUnitPlacement.Frontline) _cardsInFront.DestroyCardsOfPower(_destroyThreshold);
        if (_placement == EnumUnitPlacement.AnyPlayer || _placement == EnumUnitPlacement.Ranged) _cardsInRanged.DestroyCardsOfPower(_destroyThreshold);
        if (_placement == EnumUnitPlacement.AnyPlayer || _placement == EnumUnitPlacement.Siege) _cardsInSiege.DestroyCardsOfPower(_destroyThreshold);

        CheckForNewHighestCard();
    }

    #region Adjust Card Power Related
    public void RunEvaluationForStatChanges()
    {
        _cardsInFront.ResetCards();
        _cardsInRanged.ResetCards();
        _cardsInSiege.ResetCards();

        //weather first
        if (_effectsActive.FrontWeather) _cardsInFront.ApplyWeather();
        if (_effectsActive.RangedWeather) _cardsInRanged.ApplyWeather();
        if (_effectsActive.SiegeWeather) _cardsInSiege.ApplyWeather();

        //abilities second
        if (_effectsActive.FrontMoraleBoost) _cardsInFront.ApplyPowerFromEffect(EnumCardEffects.MoraleBoost);
        if (_effectsActive.RangedMoraleBoost) _cardsInRanged.ApplyPowerFromEffect(EnumCardEffects.MoraleBoost);
        if (_effectsActive.SiegeMoraleBoost) _cardsInSiege.ApplyPowerFromEffect(EnumCardEffects.MoraleBoost);

        //horns
        if (_effectsActive.FrontHorns) _cardsInFront.ApplyPowerFromEffect(EnumCardEffects.CommandersHorn);
        if (_effectsActive.RangedHorns) _cardsInRanged.ApplyPowerFromEffect(EnumCardEffects.CommandersHorn);
        if (_effectsActive.SiegeHorns) _cardsInSiege.ApplyPowerFromEffect(EnumCardEffects.CommandersHorn);
    }
    #endregion Adjust Card Power Related
}
