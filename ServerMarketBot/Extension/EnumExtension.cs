namespace ServerMarketBot.Extension;

public static class EnumExtension
{
    public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct, IConvertible
    {
        Type enumType = typeof(TEnum);
        return !enumType.IsEnum
            ? throw new Exception("The type could not be determined.")
            : Enum.TryParse(value, true, out TEnum val) ? val : default;
    }
}
