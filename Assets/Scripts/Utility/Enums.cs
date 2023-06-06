using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eHandSide
{
    leftHand,
    rightHand
}

public enum eItemType
{
    none,
    tool,
    container,
    pot
}

public enum eTasteType
{
    none,
    salt,
    sour,
    sweet,
}

public enum eSmellType
{
    none,
    hot
}

public enum IndicateState
{
    ENone,
    RNone,
    SpoonOut,
    Rotate,
    TasteAndSmell,
    BothNone
}

public enum InteractState
{
    None,
    Tool
}

