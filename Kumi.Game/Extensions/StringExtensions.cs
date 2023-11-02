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
}
