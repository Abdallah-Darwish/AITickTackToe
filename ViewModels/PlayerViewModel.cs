using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AITickTackToe.AI;
using AITickTackToe.Controls;
using AITickTackToe.TickTackToeGame;
using Avalonia.Media;
using Avalonia.Threading;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AITickTackToe.ViewModels
{
    public class PlayerViewModel : ReactiveObject, IDisposable
    {
        private bool _disposedValue;
        [Reactive]
        public Playground CurrentGame { get; set; }
        public char CapitalMyChar => char.ToUpper(_myChar);
        public char MyChar
        {
            get => _myChar;
            init
            {
                _myChar = value;
                _expander = new PlaygroundExpander { MyChar = value };
                _evaluator = new PlaygroundEvaluator { MyChar = value };
            }
        }
        public IBrush MyCharBrush { get; init; }
        public IBrush MyBrush => IsMyTurn ? MyCharBrush : Brushes.Black;
        [Reactive]
        public bool IsMyTurn { get; set; }
        [Reactive]
        public bool IsAutoPlayer { get; set; }
        [Reactive]
        public int AITreeDepth { get; set; }
        public PlaygroundEvaluator Evaluator => _evaluator;
        public PlaygroundExpander Expander => _expander;
        [Reactive]
        public int AIDelay { get; set; }
        public void Play()
        {
            var dn = new DecisionNode<Playground>(CurrentGame, _evaluator.Evaluate(CurrentGame));
            dn.Expand(_expander, _evaluator, AITreeDepth);
            CurrentGame = (dn.BestSon ?? dn).Value;
        }
        private readonly IDisposable[] _subs;
        private PlaygroundEvaluator _evaluator;
        private PlaygroundExpander _expander;
        private char _myChar;

        public PlayerViewModel()
        {
            _subs = new IDisposable[]
            {
                this
                .WhenAny(x => x.IsAutoPlayer, x => x.IsMyTurn, (p1, p2) => p1.Value && p2.Value)
                .Where(x => x)
                .ObserveOn(AvaloniaScheduler.Instance)
                .ForEachAsync(async _ =>
                {
                    await Task.Delay(AIDelay);
                    Play();
                })
                .ToObservable()
                .Subscribe(),
                this
                .WhenChanged(x => x.IsMyTurn, (_, p) => p)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(MyBrush)))
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                foreach (var sub in _subs)
                {
                    sub.Dispose();
                }
                _disposedValue = true;
            }
        }

        ~PlayerViewModel()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}