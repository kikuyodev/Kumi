using System.Text;
using System.Text.RegularExpressions;

namespace Kumi.Game.Extensions;

public static partial class StringExtensions
{
    public static bool IsPascalCase(this string self)
    {
        if (string.IsNullOrEmpty(self) || self.Length == 1)
            return false;
        
        if (!char.IsUpper(self[0]))
            return false;
        
        return !self.Contains('_') && !self.Contains(' ') && !self.Contains('-');
    }
    
    public static bool IsSnakeCase(this string self)
    {
        if (string.IsNullOrEmpty(self) || self.Length == 1)
            return false;

        // Check for upper case characters.
        return !self.Any(char.IsUpper);
    }
    
    public static bool IsCamelCase(this string self)
    {
        if (string.IsNullOrEmpty(self) || self.Length == 1)
            return false;
        
        if (!char.IsLower(self[0]))
            return false;
        
        return !self.Contains('_') && !self.Contains(' ') && !self.Contains('-');
    }

    public static string ToScreamingSnakeCase(this string self)
    {
        if (string.IsNullOrEmpty(self))
            return self;
        
        if (self.IsSnakeCase())
            return self.ToUpper();

        if (!self.IsCamelCase() && !self.IsPascalCase())
            return self;

        if (self.IsPascalCase())
        {
            // Lowercase the first letter for consistency.
            self = $"{char.ToLower(self[0])}{self.Substring(1)}";
        }
            
        // Split the string into words by capital letters.
        var sb = new StringBuilder();
        var words = splitByCapital().Split(self);
            
        foreach (var word in words)
        {
            sb.Append(word.ToUpper());
            sb.Append('_');
        }
            
        // Remove the trailing underscore.
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }

    [GeneratedRegex("(?<!^)(?=[A-Z])")]
    private static partial Regex splitByCapital();
}
