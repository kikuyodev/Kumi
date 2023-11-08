using JetBrains.Annotations;
using Kumi.Game.Database;
using osu.Framework.Input.Bindings;
using Realms;

namespace Kumi.Game.Input.Bindings;

/// <summary>
/// A model that stores all of the keybinds for the game.
/// </summary>
public class Keybind : RealmObject, IHasGuidPrimaryKey
{
    [PrimaryKey]
    public Guid ID { get; }

    /// <summary>
    /// The type of keybind this is.
    /// </summary>
    [Ignored]
    public KeybindType Type
    {
        get => (KeybindType)TypeInt;
        set => TypeInt = (int)value;
    }
    
    /// <summary>
    /// The key combination of this keybind.
    /// </summary>
    [Ignored]
    public KeyCombination KeyCombination
    {
        get => new KeyCombination(KeyCombinationString);
        set => KeyCombinationString = value.ToString();
    }

    /// <summary>
    /// The action that this keybind is bound to.
    /// </summary>
    public int Action { get; set; }
    
    [MapTo(nameof(KeyCombinationString))]
    public string KeyCombinationString { get; set; } = null!;

    [MapTo(nameof(Type))]
    public int TypeInt { get; set; }
    
    public Keybind(KeybindType type, int action, KeyCombination keyCombination)
    {
        ID = Guid.NewGuid();
        Type = type;
        Action = action;
        KeyCombination = keyCombination;
    }

    [UsedImplicitly]
    private Keybind()
    {
    }
    
    /// <summary>
    /// Gets the action of this keybind based on the type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetAction<T>() where T : Enum
    {
        return Type switch
        {
            KeybindType.Global => (T) Enum.ToObject(typeof(GlobalAction), Action),
            KeybindType.Gameplay => (T) Enum.ToObject(typeof(GameplayAction), Action),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum KeybindType
{
    Global,
    Gameplay
}