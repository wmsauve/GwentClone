/// <summary>
/// Determine placement for units in Gwent.
/// </summary>
public enum EnumUnitPlacement
{
    Frontline,
    Ranged,
    Siege,
    Any,
    Agile_FR,
    Agile_FS,
    Agile_RS,
    Global
}

/// <summary>
/// Differentiate Units that are placed exclusively on your side of the field or the enemies.
/// </summary>
public enum EnumUnitType
{
    Regular,
    Spy,
    NotUnit
}

/// <summary>
/// The three types of cards to be placed.
/// </summary>
public enum EnumCardType
{
    Special,
    Unit,
}

/// <summary>
/// Effects that a card can have. 
/// </summary>
public enum EnumCardEffects
{
    Decoy,
    Hero,
    Weather,
    Medic,
    Muster,
    TightBond,
    Spy,
    Berserker,
    Agile,
    CommandersHorn,
    Mardroeme,
    MoraleBoost,
    Scorch,
    None,
    Avenger
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
    Changed,
    Resolving
}

/// <summary>
/// Generally applies to any kind of animation.
/// </summary>
[System.Serializable]
public enum EnumAnimDirection
{
    Upward,
    Downward,
    Leftward,
    Rightward
}

[System.Serializable]
public enum EnumAnimEffect
{
    Fade,
    Nothing
}

public enum EnumGameplayPhases
{
    CoinFlip,
    Mulligan,
    Regular,
    GameOver
}

public enum EnumLoggerGameplay
{
    ServerProgression,
    MissingComponent,
    Error,
}

public enum EnumGameplayPlayerRole
{
    Player,
    Opponent,
}

public enum EnumMulliganPos
{
    leftout,
    left,
    leftcenter,
    center,
    rightcenter,
    right,
    rightout,
}
