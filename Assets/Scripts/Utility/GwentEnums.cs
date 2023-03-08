

namespace GwentClone
{
    /// <summary>
    /// Determine placement for units in Gwent.
    /// </summary>
    public enum EnumUnitPlacement
    {
        Frontline,
        Ranged,
        Siege
    }

    /// <summary>
    /// Differentiate Units that are placed exclusively on your side of the field or the enemies.
    /// </summary>
    public enum EnumUnitType
    {
        Regular,
        Spy
    }

    /// <summary>
    /// The three types of cards to be placed.
    /// </summary>
    public enum EnumCardType
    {
        Spell,
        Unit,
        Weather
    }

    public enum EnumFactionType
    {
        Neutral,
        Monsters,
        Nilfgaardian,
        NorthernRealms,
        Scoiatael,
        Skellige
    }

    public enum EnumDeckStatus
    {
        NotChanged,
        Changed
    }
}

