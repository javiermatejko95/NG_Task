using System;
using UnityEngine;

public static class PlayerEvents
{
    public static event Action<bool> OnToggleCanMove;

    public static void RaiseToggleCanMove(bool toggle)
    {
        OnToggleCanMove?.Invoke(toggle);
    }
}
