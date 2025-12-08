namespace SecretSantaTgBot.Utils;

public static class RandomExtension
{
    public static int[] GetShuffledUniqueIndexRange(int length)
    {
        var random = new Random();
        var result = new int[length];
        var idxs = Enumerable.Range(0, length).ToList();

        for (int i = 0; i < length; ++i)
        {
            var fi = idxs.Where(x => x != i).ToArray();
            if (fi.Length == 0)
            {
                result[i] = result[i - 1];
                result[i - 1] = idxs.Last();
                continue;
            }

            var idx = random.Next(0, fi.Length);
            result[i] = fi[idx];
            idxs.Remove(fi[idx]);
        }

        return result;
    }
}
