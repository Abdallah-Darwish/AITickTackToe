using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using AITickTackToe.AI;
using AITickTackToe.AI.Rendering;
using AITickTackToe.Controls;
using AITickTackToe.XOGame.Rendering;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AITickTackToe.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        public void Reset()
        {
            Player1.IsAutoPlayer = Player2.IsAutoPlayer = false;
            PlaygroundControl.Value = new Playground();
            PlaygroundControl.Version = 0;
        }
        public PlayerViewModel Player1 { get; init; }
        public PlayerViewModel Player2 { get; init; }
        public DecisionNodeRenderingConfig<Playground> RenderingConfig { get; init; }
        public XOPlaygroundControl PlaygroundControl { get; init; }
        [Reactive]
        public IBitmap DecisionTree { get; set; }

        public void UpdateDecisionTree(object? sender, Avalonia.Input.GotFocusEventArgs e)
        {
            var p = Player1.IsMyTurn ? Player1 : Player2;
            var dn = new DecisionNode<Playground>(p.CurrentGame, p.Evaluator.Evaluate(p.CurrentGame));
            dn.Expand(p.Expander, p.Evaluator, p.AITreeDepth);
            var renderer = new DecisionNodeRenderer<Playground>(dn, new SimplePlaygroundRenderer { Config = RenderingConfig.ValueConfig as PlaygroundRenderingConfig }, RenderingConfig);

            var img = new RenderTargetBitmap(new PixelSize((int)renderer.Root.SubTreeBounds.Width + 2, (int)renderer.Root.SubTreeBounds.Height + 2));
            using (var ctx = new Avalonia.Media.DrawingContext(img.CreateDrawingContext(null), true))
            {
                renderer.Draw(ctx);
            }
            DecisionTree.Dispose();
            DecisionTree = img;
        }

        private IDisposable[] _subs;
        private bool _disposedValue;

        public void Init()
        {
            DecisionTree = new RenderTargetBitmap(new PixelSize(100, 100));
            _subs = new IDisposable[]
            {
                PlaygroundControl.GetPropertyChangedObservable(XOPlaygroundControl.ValueProperty)
                .Select(e => (e.NewValue as Playground)!)
                .Merge(Player1.ObservableForProperty(x => x.CurrentGame).Select(x => x.Value))
                .Merge(Player2.ObservableForProperty(x => x.CurrentGame).Select(x => x.Value))
                .DistinctUntilChanged()
                .Prepend(new Playground())
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(v =>
                {
                    PlaygroundControl.Value = Player1.CurrentGame = Player2.CurrentGame = v;
                    Dispatcher.UIThread.RunJobs();
                }),
                PlaygroundControl.GetPropertyChangedObservable(XOPlaygroundControl.IsFirstPlayerTurnProperty)
                .Prepend(new AvaloniaPropertyChangedEventArgs(null, null, false, true, Avalonia.Data.BindingPriority.Unset))
                .Subscribe(e =>
                {
                    if(PlaygroundControl.IsGameDone)
                    {
                        Player1.IsMyTurn = Player2.IsMyTurn = false;
                        return;
                    }
                    var v = (bool)e.NewValue;
                    Player1.IsMyTurn = v;
                    Player2.IsMyTurn = !v;
                    Dispatcher.UIThread.RunJobs();
                }),
            };
            PlaygroundControl.Version = 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                foreach (var sub in _subs)
                {
                    sub.Dispose();
                }
                Player1.Dispose();
                Player2.Dispose();
                _disposedValue = true;
            }
        }

        ~MainWindowViewModel()
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
