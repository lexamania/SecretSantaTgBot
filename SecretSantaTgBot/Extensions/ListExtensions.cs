namespace SecretSantaTgBot.Extensions;

public static class ListExtensions
{
    extension<T>(List<T> source)
    {
        public T RemoveFirst(Func<T, bool> predicate)
        {
            var item = source.First(predicate);
            source.Remove(item);
            return item;
        }
    }
}
