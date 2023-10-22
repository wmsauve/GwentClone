using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class S_GameZones
{
    public class GameZone
    {
        public List<GwentCard> Cards = new List<GwentCard>();
        public int TotalPower;

        public List<GwentCard> HighestPowerCards = new List<GwentCard>();
        public int HighestPowerCard;
        public int FlaggedPowerForDestroy;

        private bool _weather = false;
        private bool _horn = false;
        private int _moraleBoosts = 0;

        public bool WeatherActive 
        {
            get
            {
                return _weather;
            }
            set
            {
                _weather = value;
            }
        }
        public bool HornActive
        {
            get
            {
                return _horn;
            }
            set
            {
                _horn = value;
            }
        }
        
        public void AddCardToZone(GwentCard _newCard)
        {
            Cards.Add(_newCard);
            RunHighestCardCheck();
        }

        public void DestroyCardsOfPower(int power)
        {
            FlaggedPowerForDestroy = power;
            List<GwentCard> savedCards = new List<GwentCard>();
            foreach(GwentCard _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero) || _card.cardPower != power) savedCards.Add(_card);
            }

            Cards = savedCards;
            RunHighestCardCheck();
        }

        public void RunMoraleBoostCheck()
        {
            foreach (GwentCard _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.MoraleBoost)) _moraleBoosts++;
            }
        }

        private void ApplyMoraleBoost()
        {
            foreach (GwentCard _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero) || _card.cardEffects.Contains(EnumCardEffects.MoraleBoost)) continue;
                _card.cardPower += _moraleBoosts;
            }
        }

        private void ApplyWeather()
        {
            foreach(GwentCard _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero)) continue;
                _card.cardPower = 1;
            }
        }

        private void ApplyHorn()
        {
            foreach (GwentCard _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero) && _card.cardEffects.Contains(EnumCardEffects.CommandersHorn)) continue;
                _card.cardPower *= 2;
            }
        }

        private void ResetCards()
        {
            foreach (GwentCard _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero)) continue;
                _card.ResetToBasePower();
            }
        }

        public void RunStatChangeCheck()
        {
            ResetCards();
            if (_weather) ApplyWeather();
            if (_horn) ApplyHorn();
            ApplyMoraleBoost();
            RunHighestCardCheck();
        }

        private void RunHighestCardCheck()
        {
            TotalPower = 0;
            HighestPowerCard = 0;
            HighestPowerCards.Clear();

            if (Cards == null || Cards.Count == 0) return;

            foreach (GwentCard _card in Cards)
            {
                if (_card.cardEffects.Contains(EnumCardEffects.Hero)) continue;

                TotalPower += _card.cardPower;

                if (_card.cardPower > HighestPowerCard)
                {
                    HighestPowerCard = _card.cardPower;
                    HighestPowerCards = new List<GwentCard>();
                    HighestPowerCards.Add(_card);
                }

                else if (_card.cardPower == HighestPowerCard) HighestPowerCards.Add(_card);
            }
        }
    }

    private GameZone _cardsInFront = new GameZone();
    private GameZone _cardsInRanged = new GameZone();
    private GameZone _cardsInSiege = new GameZone();

    public GameZone CardsInFront { get { return _cardsInFront; } }
    public GameZone CardsInRanged { get { return _cardsInRanged; } }
    public GameZone CardsInSiege { get { return _cardsInSiege; } }

    private List<GwentCard> _highestPowerCards = new List<GwentCard>();
    public List<GwentCard> HighestPowerCard { get { return _highestPowerCards; } }

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

    public void ActivateStatChangeRow(EnumUnitPlacement _placement, EnumCardEffects _which)
    {
        GameZone _zone = null;
        switch (_placement)
        {
            case EnumUnitPlacement.Frontline: _zone = _cardsInFront; break;
            case EnumUnitPlacement.Ranged: _zone = _cardsInRanged; break;
            case EnumUnitPlacement.Siege: _zone = _cardsInSiege; break;
            default:
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Wrong zone for this method.");
                break;
        }

        if (_zone == null) return;

        switch (_which)
        {
            case EnumCardEffects.CommandersHorn: _zone.HornActive = true; break;
            case EnumCardEffects.MoraleBoost: _zone.RunMoraleBoostCheck(); break;
            case EnumCardEffects.Weather: _zone.WeatherActive = true; break;
            default:
                GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Wrong effect for this method.");
                break;
        }
    }

    public void UpdateScoresRelated()
    {
        _cardsInFront.RunStatChangeCheck();
        _cardsInRanged.RunStatChangeCheck();
        _cardsInSiege.RunStatChangeCheck();
    }

    public void ActivateStatChangeSingle()
    {

    }

    #endregion Adjust Card Power Related
}
