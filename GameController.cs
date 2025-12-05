using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MGLibrary;
using MGLibrary.Input;

namespace BatSmasher;

/// <summary>
/// Provides a game-specific input abstraction that maps physical inputs
/// to game actions, bridging our input system with game-specific functionality.
/// </summary>
public static class GameController
{
    private static KeyboardInfo s_keyboard => Core.Input.Keyboard;
    /// <summary>
    /// Returns true if the player has triggered the "move up" action.
    /// </summary>
    public static bool MoveUp()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Up) ||
               s_keyboard.WasKeyJustPressed(Keys.W);
    }

    /// <summary>
    /// Returns true if the player has triggered the "move down" action.
    /// </summary>
    public static bool MoveDown()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Down) ||
               s_keyboard.WasKeyJustPressed(Keys.S);
    }

    /// <summary>
    /// Returns true if the player has triggered the "move left" action.
    /// </summary>
    public static bool MoveLeft()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Left) ||
               s_keyboard.WasKeyJustPressed(Keys.A);
    }

    /// <summary>
    /// Returns true if the player has triggered the "move right" action.
    /// </summary>
    public static bool MoveRight()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Right) ||
               s_keyboard.WasKeyJustPressed(Keys.D);
    }

    /// <summary>
    /// Returns true if the player has triggered the "pause" action.
    /// </summary>
    public static bool Pause()
    {
        return s_keyboard.WasKeyJustPressed(Keys.Escape);
    }

    /// <summary>
    /// Returns true if the player has triggered the "action" button,
    /// typically used for menu confirmation.
    /// </summary>
    public static bool Action()
    {

        return s_keyboard.WasKeyJustPressed(Keys.Enter);
    }
}

