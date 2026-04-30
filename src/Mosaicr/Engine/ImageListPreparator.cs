namespace Mosaicr.Engine;

public static class ImageListPreparator
{
    public static List<string?> Prepare(List<string> tileFiles, int tilesNeeded, bool shuffleEnabled, bool fillWithImages)
    {
        var result = new List<string?>(tileFiles);

        if (shuffleEnabled)
        {
            Shuffle(result);
        }

        var originalCount = result.Count;
        var index = 0;

        while (result.Count < tilesNeeded)
        {
            if (fillWithImages)
            {
                result.Add(result[index % originalCount]);
                index++;
            }
            else
            {
                result.Add(null);
            }
        }

        if (shuffleEnabled)
        {
            Shuffle(result);
        }

        return result;
    }

    private static void Shuffle<T>(List<T> list)
    {
        var rng = Random.Shared;
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
