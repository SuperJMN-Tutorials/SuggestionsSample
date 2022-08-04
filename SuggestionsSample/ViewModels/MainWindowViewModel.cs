using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SuggestionsSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<IList<Suggestion>> suggestions;

        public MainWindowViewModel()
        {
            Transactions = new[]
            {
                new Transaction(1),
                new Transaction(2),
                new Transaction(3),
            };

            suggestions = this.WhenAnyValue(x => x.SelectedTransaction)
                .WhereNotNull()
                .Select(GetSuggestions)
                .Switch()
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.Suggestions);
        }

        public IList<Suggestion> Suggestions => suggestions.Value;

        private IObservable<IList<Suggestion>> GetSuggestions(Transaction transaction)
        {
            return Observable
                .Range(0, Random.Shared.Next(1, 5))
                .Select(index => new Suggestion(Random.Shared.Next(0, 1000) * transaction.Id + index))
                .Delay(TimeSpan.FromSeconds(Random.Shared.Next(1, 3)))
                .ToList();
        }

        public Transaction[] Transactions { get; }

        [Reactive]
        public Transaction SelectedTransaction { get; set; }
    }

    public class Suggestion
    {
        public int Id { get; }

        public Suggestion(int id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return $"Suggestion " + Id;
        }
    }

    public class Transaction
    {
        public int Id { get; }

        public Transaction(int id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return $"Transaction " + Id;
        }
    }
}
