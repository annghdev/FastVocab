using FastVocab.Shared.DTOs.Words;

namespace FastVocab.BlazorWebApp.Pages.CardMatchGames;

public static class CardMatchGenerator
{
    public static List<List<CardItem>> GenerateBatches(List<WordDto> words, int cardsPerBatch = 8)
    {
        var pairsPerBatch = cardsPerBatch / 2;
        var shuffled = words.OrderBy(_ => Guid.NewGuid()).ToList();
        var batches = new List<List<CardItem>>();

        for (int i = 0; i < shuffled.Count; i += pairsPerBatch)
        {
            var slice = shuffled.Skip(i).Take(pairsPerBatch).ToList();
            if (!slice.Any()) break;

            var cards = new List<CardItem>();
            foreach (var w in slice)
            {
                cards.Add(new CardItem { PairId = w.Id, DisplayText = w.Text, Type = CardType.Word });
                cards.Add(new CardItem { PairId = w.Id, DisplayText = w.Meaning, Type = CardType.Meaning });
            }

            batches.Add(cards.OrderBy(_ => Guid.NewGuid()).ToList());
        }

        return batches;
    }
}

public enum CardType { Word, Meaning }

public class CardItem
{
    public int PairId { get; set; }
    public string DisplayText { get; set; } = "";
    public CardType Type { get; set; }
    public string Status { get; set; } = "";  // "", selected, correct, wrong
    public bool IsHidden { get; set; } = false;
}
