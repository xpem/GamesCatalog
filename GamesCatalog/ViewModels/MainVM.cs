using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamesCatalog.Models;
using GamesCatalog.Views.Game;
using GamesCatalog.Views.Game.IGDBSearch;
using Models;
using Models.DTOs;
using Services;
using System.Collections.ObjectModel;

namespace GamesCatalog.ViewModels;

public partial class MainVM(IGameService gameService) : ViewModelBase
{

    private ObservableCollection<UIHorizontalColViewGroupedImage> lastFiveIGDBIdsByUpdatedAtWant = [],
        lastFiveIGDBIdsByUpdatedAtPlaying = [], lastFiveIGDBIdsByUpdatedAtPlayed = [];

    public ObservableCollection<UIHorizontalColViewGroupedImage> LastFiveIGDBIdsByUpdatedAtWant
    {
        get => lastFiveIGDBIdsByUpdatedAtWant;
        set { if (value != lastFiveIGDBIdsByUpdatedAtWant) SetProperty(ref (lastFiveIGDBIdsByUpdatedAtWant), value); }
    }

    public ObservableCollection<UIHorizontalColViewGroupedImage> LastFiveIGDBIdsByUpdatedAtPlaying
    {
        get => lastFiveIGDBIdsByUpdatedAtPlaying;
        set { if (value != lastFiveIGDBIdsByUpdatedAtPlaying) SetProperty(ref (lastFiveIGDBIdsByUpdatedAtPlaying), value); }
    }

    public ObservableCollection<UIHorizontalColViewGroupedImage> LastFiveIGDBIdsByUpdatedAtPlayed
    {
        get => lastFiveIGDBIdsByUpdatedAtPlayed;
        set { if (value != lastFiveIGDBIdsByUpdatedAtPlayed) SetProperty(ref (lastFiveIGDBIdsByUpdatedAtPlayed), value); }
    }

    private string?[] CacheLastFiveIGDBIdsByUpdatedAtWant = [], CacheLastFiveIGDBIdsByUpdatedAtPlaying = [], CacheLastFiveIGDBIdsByUpdatedAtPlayed = [];

    private bool btnAddGameIsEnabled = true;

    public bool BtnAddGameIsEnabled { get => btnAddGameIsEnabled; set { if (value != btnAddGameIsEnabled) { SetProperty(ref (btnAddGameIsEnabled), value); } } }

    private Color isConnectedColor = Colors.Green;

    public Color IsConnectedColor { get => isConnectedColor; set { if (value != isConnectedColor) { SetProperty(ref (isConnectedColor), value); } } }

    private int qtdWant = 0, qtdPlaying = 0, qtdPlayed = 0;

    public int QtdWant
    {
        get => qtdWant;
        set => SetProperty(ref qtdWant, value);
    }

    public int QtdPlaying
    {
        get => qtdPlaying;
        set => SetProperty(ref qtdPlaying, value);
    }

    public int QtdPlayed
    {
        get => qtdPlayed;
        set => SetProperty(ref qtdPlayed, value);
    }

    [RelayCommand]
    private Task GoToIGDBResults() => Shell.Current.GoToAsync($"{nameof(IGDBResults)}");

    [RelayCommand]
    private async Task Appearing()
    {
        if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
        {
            IsConnectedColor = Colors.Red;
            BtnAddGameIsEnabled = false;
        }
        else
        {
            IsConnectedColor = Colors.Green;
            BtnAddGameIsEnabled = true;
        }

        _ = SetTotalsGroupedByStatus();
    }

    private readonly SemaphoreSlim SetTotalsGroupedByStatusSemaphore = new(1, 1);

    public async Task SetTotalsGroupedByStatus()
    {
        await SetTotalsGroupedByStatusSemaphore.WaitAsync();

        try
        {
            List<TotalGroupedByStatus>? totalsGroupedByStatus = gameService.GetTotalsGroupedByStatus();

            if (totalsGroupedByStatus is not null && totalsGroupedByStatus.Count > 0)
            {
                List<Task> tasks = [
                    Task.Run(() => BuildColView(totalsGroupedByStatus, GameStatus.Want, ref lastFiveIGDBIdsByUpdatedAtWant, ref CacheLastFiveIGDBIdsByUpdatedAtWant)),
                    Task.Run(() => BuildColView(totalsGroupedByStatus, GameStatus.Playing, ref lastFiveIGDBIdsByUpdatedAtPlaying, ref CacheLastFiveIGDBIdsByUpdatedAtPlaying)),
                    Task.Run(() => BuildColView(totalsGroupedByStatus, GameStatus.Played, ref lastFiveIGDBIdsByUpdatedAtPlayed, ref CacheLastFiveIGDBIdsByUpdatedAtPlayed))
                ];

                await Task.WhenAll(tasks);
            }
            else
            {
                QtdPlayed = QtdPlaying = QtdWant = 0;

                MainVM.ClearColView(ref lastFiveIGDBIdsByUpdatedAtWant);
                MainVM.ClearColView(ref lastFiveIGDBIdsByUpdatedAtPlaying);
                MainVM.ClearColView(ref lastFiveIGDBIdsByUpdatedAtPlayed);
            }
        }
        catch (Exception ex) { throw ex; }
        finally
        {
            SetTotalsGroupedByStatusSemaphore.Release();
        }
    }

    private static void ClearColView(ref ObservableCollection<UIHorizontalColViewGroupedImage> lastFiveIGDBIdsByUpdatedAt)
    {
        while (lastFiveIGDBIdsByUpdatedAt.Count > 0)
            lastFiveIGDBIdsByUpdatedAt.RemoveAt(lastFiveIGDBIdsByUpdatedAt.Count - 1);
    }

    public void BuildColView(List<TotalGroupedByStatus> totalsGroupedByStatus, GameStatus gameStatus,
        ref ObservableCollection<UIHorizontalColViewGroupedImage> lastFiveIGDBIdsByUpdatedAt, ref string?[] lastFiveIGDBIdsByUpdatedAtCache)
    {
        TotalGroupedByStatus? totalsGrouped = totalsGroupedByStatus.FirstOrDefault(x => x.Status == gameStatus);

        switch (gameStatus)
        {
            case GameStatus.Want:
                QtdWant = totalsGrouped?.Total ?? 0;
                break;
            case GameStatus.Playing:
                QtdPlaying = totalsGrouped?.Total ?? 0;
                break;
            case GameStatus.Played:
                QtdPlayed = totalsGrouped?.Total ?? 0;
                break;
        }

        string?[] _lastFiveIGDBIdsByUpdatedAt = totalsGrouped?.LastFiveIGDBIdsByUpdatedAt ?? [];

        if (_lastFiveIGDBIdsByUpdatedAt is null or [])
        {
            MainVM.ClearColView(ref lastFiveIGDBIdsByUpdatedAt);

            lastFiveIGDBIdsByUpdatedAtCache = [];

            return;
        }

        if (!lastFiveIGDBIdsByUpdatedAtCache.SequenceEqual(_lastFiveIGDBIdsByUpdatedAt))
        {
            MainVM.ClearColView(ref lastFiveIGDBIdsByUpdatedAt);

            bool isFisrt = true;

            foreach (var item in _lastFiveIGDBIdsByUpdatedAt)
            {
                if (isFisrt)
                {
                    lastFiveIGDBIdsByUpdatedAt.Add(new UIHorizontalColViewGroupedImage(Path.Combine(GameService.ImagesPath, $"{item}.jpg"), new Thickness(0)));
                    isFisrt = false;
                }
                else
                    lastFiveIGDBIdsByUpdatedAt.Add(new UIHorizontalColViewGroupedImage(Path.Combine(GameService.ImagesPath, $"{item}.jpg"), new Thickness(-45, 0, 0, 0)));
            }
        }

        lastFiveIGDBIdsByUpdatedAtCache = _lastFiveIGDBIdsByUpdatedAt;
    }

    [RelayCommand]
    private Task WantList() => GoToAsync($"{nameof(GameList)}?GameStatus={(int)GameStatus.Want}");

    [RelayCommand]
    private Task PlayingList() => GoToAsync($"{nameof(GameList)}?GameStatus={(int)GameStatus.Playing}");

    [RelayCommand]
    private Task PlayedList() => GoToAsync($"{nameof(GameList)}?GameStatus={(int)GameStatus.Played}");

    private static Task GoToAsync(string route)
    {
        return Shell.Current.GoToAsync(route, true);
    }
}
