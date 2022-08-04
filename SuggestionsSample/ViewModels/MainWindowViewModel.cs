using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
                .Select(x => WithProgress(() => ConcurrentCount++, () => ConcurrentCount--, () => GetSuggestions(x)))                
                .Switch()
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.Suggestions);

            this.WhenAnyValue(x => x.ConcurrentCount).Select(x => x > 0).ToPropertyEx(this, model => model.IsBusy);
        }

        [ObservableAsProperty]
        public bool IsBusy { get; }

        [Reactive]
        public int ConcurrentCount { get; set; }

        public IList<Suggestion> Suggestions => suggestions.Value;

        private async Task<IList<Suggestion>> GetSuggestions(Transaction transaction)
        {
            return await Observable
                .Range(0, Random.Shared.Next(1, 5))
                .Select(index => new Suggestion(Random.Shared.Next(0, 1000) * transaction.Id + index))
                .Delay(TimeSpan.FromSeconds(Random.Shared.Next(1, 3)))
                .ToList();
        }

        public Transaction[] Transactions { get; }

        [Reactive]
        public Transaction SelectedTransaction { get; set; }

        public static async Task<T> WithProgress<T>(Action addRef, Action release, Func<Task<T>> block) {
            RxApp.MainThreadScheduler.Schedule(addRef);

            try { 
                return await block();
            } finally {
                RxApp.MainThreadScheduler.Schedule(release);
            }
        }
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