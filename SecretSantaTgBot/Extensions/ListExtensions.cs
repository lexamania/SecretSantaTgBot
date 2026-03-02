namespace SecretSantaTgBot.Extensions;

public static class ListExtensions
{
    public static T RemoveFirst<T>(this List<T> items, Func<T, bool> predicate)
    {
        var item = items.First(predicate);
        items.Remove(item);
        return item;
    }
}
