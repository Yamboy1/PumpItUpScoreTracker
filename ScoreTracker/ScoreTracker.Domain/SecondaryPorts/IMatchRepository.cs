﻿using ScoreTracker.Domain.Models;
using ScoreTracker.Domain.ValueTypes;
using ScoreTracker.Domain.Views;

namespace ScoreTracker.Domain.SecondaryPorts;

public interface IMatchRepository
{
    Task<MatchView> GetMatch(Name matchName, CancellationToken cancellationToken);
    Task<IEnumerable<MatchView>> GetAllMatches(CancellationToken cancellationToken);
    Task SaveMatch(MatchView matchView, CancellationToken cancellationToken);
    Task SaveRandomSettings(Name settingsName, RandomSettings settings, CancellationToken cancellationToken);
    Task<RandomSettings> GetRandomSettings(Name settingsName, CancellationToken cancellationToken);

    Task<IEnumerable<(Name name, RandomSettings settings)>> GetAllRandomSettings(
        CancellationToken cancellationToken);
}