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
    Global,
    SingleTarget
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
    Nothing,
    Upward,
    Downward,
    Leftward,
    Rightward,
    GrowOut,
    GrowIn,
}

public enum EnumGameplayPhases
{
    CoinFlip,
    Mulligan,
    Regular,
    MatchOver,
    GameOver
}

public enum EnumLoggerGameplay
{
    ServerProgression,
    MissingComponent,
    Error,
    InvalidInput,
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

public enum EnumPlayCardReason
{
    ClickCard,
    ClickZone,
    ClickEnemyZone,
    SingleTarget,
    SingleTargetEnemy,
}

public enum EnumCardListType
{
    Hand,
    Graveyard,
}
