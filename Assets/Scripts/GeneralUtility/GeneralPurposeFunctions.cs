using UnityEngine;
using System;

public class GeneralPurposeFunctions
{
    private static string EFFECT_DECOY = "Swap with a card on the battlefield to return it to your hand.";
    private static string EFFECT_HERO = "Not affected by any Special Cards or abilities.";
    private static string EFFECT_MEDIC = "Choose one card from your discard pile and play it instantly (no Heroes or Special Cards).";
    private static string EFFECT_MUSTER = " Find any cards with the same name in your deck and play them instantly.";
    private static string EFFECT_TIGHTBOND = "When placed next to a card with the same name, doubles the strength of both (or more) cards.";
    private static string EFFECT_SPY = "Can be placed on your opponent's battlefield (and count towards your opponent's total) but allow you to draw 2 extra cards from your deck.";
    private static string EFFECT_MORALEBOOST = "Add +1 strength to all units in the row in which they are played, excluding themselves.";
    private static string EFFECT_AGILE = "Can be placed in either the Close Combat or the Ranged Combat row. Cannot be moved once placed.";
    private static string EFFECT_SCORCHRANGED = "Destroy your enemy's strongest Ranged Combat unit(s) if the combined strength of all his or her Ranged Combat units is 10 or more.";
    private static string EFFECT_CLOSECOMBATWEATHER = "Sets the strength of all Close Combat cards to 1 for both players.";
    public static Color ReturnColorBasedOnFaction(EnumFactionType faction)
    {
        switch (faction)
        {
            case EnumFactionType.Monsters:
                return Color.red;
            case EnumFactionType.Neutral:
                return new Color(0f, 0f, 0f, 0f);
            case EnumFactionType.Nilfgaardian:
                return Color.black;
            case EnumFactionType.NorthernRealms:
                return Color.blue;
            case EnumFactionType.Scoiatael:
                return Color.green;
            case EnumFactionType.Skellige:
                return Color.magenta;
            default:
                return new Color(0f, 0f, 0f, 0f);
        }
    }

    public static string ReturnSkillDescription(EnumCardEffects effect, EnumUnitPlacement placement = EnumUnitPlacement.AnyPlayer)
    {
        switch (effect)
        {
            case EnumCardEffects.Decoy:
                return EFFECT_DECOY;
            case EnumCardEffects.Hero:
                return EFFECT_HERO;
            case EnumCardEffects.Medic:
                return EFFECT_MEDIC;
            case EnumCardEffects.Muster:
                return EFFECT_MUSTER;
            case EnumCardEffects.TightBond:
                return EFFECT_TIGHTBOND;
            case EnumCardEffects.Spy:
                return EFFECT_SPY;
            case EnumCardEffects.MoraleBoost:
                return EFFECT_MORALEBOOST;
            case EnumCardEffects.Agile:
                return EFFECT_AGILE;
            case EnumCardEffects.Scorch:
                if (placement == EnumUnitPlacement.Ranged) return EFFECT_SCORCHRANGED;
                return "";
            case EnumCardEffects.Weather:
                if (placement == EnumUnitPlacement.Frontline) return EFFECT_CLOSECOMBATWEATHER;
                return "";
            case EnumCardEffects.None:
                return "";
            default:
                return "";
        }
    }

    /// <summary>
    /// Username can be used to denoted who is sending invalid inputs.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="message"></param>
    /// <param name="username"></param>
    public static void GamePlayLogger(EnumLoggerGameplay log, string message, string username = "")
    {
        switch (log)
        {
            case EnumLoggerGameplay.ServerProgression:
                Debug.Log("Gameplay Progression: " + message);
                break;
            case EnumLoggerGameplay.MissingComponent:
                Debug.LogError("Component Error: " + message);
                break;
            case EnumLoggerGameplay.Error:
                Debug.LogError("Logic Error: " + message);
                break;
            case EnumLoggerGameplay.InvalidInput:
                Debug.LogError($"Invalid Input From {username}: " + message);
                break;
            default:
                Debug.LogWarning("Error: Figure out what went long with gameplay logging.");
                break;
        }
    }

    public static void EnableAllChildrenObjects(GameObject setActive)
    {
        if (!setActive.activeSelf)
        {
            setActive.SetActive(true);
        }

        if (setActive.transform.childCount == 0)
        {
            return;
        }

        for (int i = 0; i < setActive.transform.childCount; i++)
        {
            EnableAllChildrenObjects(setActive.transform.GetChild(i).gameObject);
        }
    }

    public static int FindIndexByPropertyValue<T>(T[] array, Func<T, bool> predicate)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (predicate(array[i]))
            {
                return i; // Found a match, return the index
            }
        }

        return -1; // No match found
    }


    [Serializable]
    public struct ArrayWrapper<T>
    {
        public T[] array;
    }
    public static string ConvertArrayToJson<T>(T[] arr)
    {
        ArrayWrapper<T> wrapper = new ArrayWrapper<T>
        {
            array = arr
        };

        return JsonUtility.ToJson(wrapper);
    }

    public static C_PlayerGamePlayLogic GetPlayerLogicReference()
    {

        var logics = UnityEngine.Object.FindObjectsOfType<C_PlayerGamePlayLogic>();
        if (logics.Length == 0) GeneralPurposeFunctions.GamePlayLogger(EnumLoggerGameplay.Error, "Should have player logic");
        for (int i = 0; i < logics.Length; i++)
        {
            if (logics[i].ReturnOwnerStatus())
            {
                return logics[i];
            }
        }

        GamePlayLogger(EnumLoggerGameplay.Error, "Can't find player logic for some reason.");
        return null;
        
    }

    /// <summary>
    /// gameObject is where you are looking for the component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T GetComponentFromGameObject<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            string typeName = typeof(T).Name;
            string errorMessage = $"Failed to find {typeName} component on the GameObject.";
            GamePlayLogger(EnumLoggerGameplay.MissingComponent, errorMessage);
        }
        return component;
    }

    public static T GetComponentFromScene<T>(GameObject prefab) where T : Component
    {
        T[] objects = GameObject.FindObjectsOfType<T>();

        if (objects.Length == 0)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            return obj.GetComponent<T>();
        }

        if (objects.Length > 1)
        {
            GamePlayLogger(EnumLoggerGameplay.Error, "You don't need multiple instances of " + typeof(T).Name);
            return null;
        }

        return objects[0];
    }

    public static T[] GetComponentsFromScene<T>() where T: Component
    {
        T[] objects = GameObject.FindObjectsOfType<T>();

        if (objects == null || objects.Length == 0)
        {
            GamePlayLogger(EnumLoggerGameplay.Error, "You don't have any components of type: " + typeof(T).Name);
            return null;
        }

        return objects;
    }

    #region Game Logic Related

    public static bool PlayCardOnDrop(EnumPlayCardStatus desired, EnumUnitPlacement _card)
    {
        if(desired == EnumPlayCardStatus.PlayToZone)
        {
            if (_card == EnumUnitPlacement.Global) return false;
            if (_card == EnumUnitPlacement.SingleTarget) return false;

            return true;
        }

        if(desired == EnumPlayCardStatus.SingleTarget)
        {
            if (_card != EnumUnitPlacement.SingleTarget) return false;

            return true;
        }

        if(desired == EnumPlayCardStatus.Global)
        {
            if (_card != EnumUnitPlacement.Global) return false;

            return true;
        }

        return false;
    }

    public static EnumPlayCardStatus GetIntendedPlayLocation(Card _data)
    {
        //For now. need to update this later for single target cards.
        switch (_data.unitPlacement)
        {
            case EnumUnitPlacement.Frontline:
            case EnumUnitPlacement.Ranged:
            case EnumUnitPlacement.Siege:
            case EnumUnitPlacement.Agile_FR:
            case EnumUnitPlacement.Agile_FS:
            case EnumUnitPlacement.Agile_RS:
                return EnumPlayCardStatus.PlayToZone;
            case EnumUnitPlacement.Global:
                return EnumPlayCardStatus.Global;
            case EnumUnitPlacement.SingleTarget:
                return EnumPlayCardStatus.SingleTarget;
        }
        GamePlayLogger(EnumLoggerGameplay.Error, "You should not reach this point for setting Intended play location for dropped card.");
        return EnumPlayCardStatus.PlayToZone;
    }

    #endregion Game Logic Related
}
