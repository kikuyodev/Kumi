using System.Text;
using System.Text.RegularExpressions;

namespace Kumi.Game.Extensions;

public static class StringExtensions
{
    public static bool IsPascalCase(this string self)
    {
        if (string.IsNullOrEmpty(self) || self.Length == 1)
            return false;
        
        if (!char.IsUpper(self[0]))
            return false;
        
        if (self.IndexOf('_') != -1 || self.IndexOf(' ') != -1 || self.IndexOf('-') != -1)
            return false;

        return true;
    }
    
    public static bool IsSnakeCase(this string self)
    {
        if (string.IsNullOrEmpty(self) || self.Length == 1)
            return false;
        
        // Check for upper case characters.
        if (self.Any(char.IsUpper))
            return false;

        return true;
    }
    
    public static bool IsCamelCase(this string self)
    {
        if (string.IsNullOrEmpty(self) || self.Length == 1)
            return false;
        
        if (!char.IsLower(self[0]))
            return false;
        
        if (self.IndexOf('_') != -1 || self.IndexOf(' ') != -1 || self.IndexOf('-') != -1)
            return false;

        return true;
    }

    public static string ToScreamingSnakeCase(this string self)
    {
        if (string.IsNullOrEmpty(self))
            return self;
        
        if (self.IsSnakeCase())
            return self.ToUpper();

        if (self.IsCamelCase() || self.IsPascalCase())
        {
            if (self.IsPascalCase())
            {
                // Lowercase the first letter for consistency.
                self = $"{char.ToLower(self[0])}{self.Substring(1)}";
            }
            
            // Split the string into words by capital letters.
            var sb = new StringBuilder();
            var words = Regex.Split(self, @"(?<!^)(?=[A-Z])");
            
            foreach (var word in words)
            {
                sb.Append(word.ToUpper());
                sb.Append('_');
            }
            
            // Remove the trailing underscore.
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        return self;
    }
}
