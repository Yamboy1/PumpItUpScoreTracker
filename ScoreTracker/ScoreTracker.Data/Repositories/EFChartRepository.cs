﻿using Microsoft.EntityFrameworkCore;
using ScoreTracker.Data.Persistence;
using ScoreTracker.Data.Persistence.Entities;
using ScoreTracker.Domain.Enums;
using ScoreTracker.Domain.Exceptions;
using ScoreTracker.Domain.Models;
using ScoreTracker.Domain.SecondaryPorts;
using ScoreTracker.Domain.ValueTypes;

namespace ScoreTracker.Data.Repositories;

public sealed class EFChartRepository : IChartRepository
{
    private readonly ChartAttemptDbContext _database;

    public EFChartRepository(ChartAttemptDbContext database)
    {
        _database = database;
    }

    public async Task<IDictionary<Name, int>> GetSongOrder(CancellationToken cancellationToken = default)
    {
        return await (
                from so in _database.SongOrder
                join s in _database.Song on so.SongId equals s.Id
                select new { s.Name, so.Order })
            .ToDictionaryAsync(q => (Name)q.Name, q => q.Order, cancellationToken);
    }

    public async Task<IEnumerable<Chart>> GetCharts(IEnumerable<DifficultyLevel>? levels = default,
        IEnumerable<ChartType>? chartTypes = default, string? songNameContains = default,
        CancellationToken cancellationToken = default)
    {
        var query = from c in _database.Chart
            join s in _database.Song on c.SongId equals s.Id
            select new { c, s };
        if (levels != null)
        {
            var intLevels = levels.Select(l => (int)l).ToArray();
            query = query.Where(q => intLevels.Contains(q.c.Level));
        }

        if (chartTypes != null)
        {
            var stringTypes = chartTypes.Select(c => c.ToString()).ToArray();
            query = query.Where(q => stringTypes.Contains(q.c.Type));
        }

        if (songNameContains != null) query = query.Where(q => q.s.Name.Contains(songNameContains));

        return await (from q in query
            select new Chart(q.c.Id, new Song(q.s.Name, new Uri(q.s.ImagePath)), Enum.Parse<ChartType>(q.c.Type),
                q.c.Level)).ToArrayAsync(cancellationToken);
    }

    public async Task<IEnumerable<Name>> GetSongNames(CancellationToken cancellationToken = default)
    {
        return (await _database.Song.Select(s => s.Name).ToArrayAsync(cancellationToken)).Select(Name.From);
    }

    public async Task<Chart> GetChart(Name songName, ChartType chartType, DifficultyLevel level,
        CancellationToken cancellationToken = default)
    {
        if (!await _database.Song.AnyAsync(s => s.Name == (string)songName, cancellationToken))
            throw new SongNotFoundException();
        return await (from s in _database.Song
                join c in _database.Chart on s.Id equals c.SongId
                where s.Name == (string)songName && c.Type == chartType.ToString() && c.Level == (int)level
                select new Chart(c.Id, new Song(s.Name, new Uri(s.ImagePath)), Enum.Parse<ChartType>(c.Type), c.Level))
            .FirstOrDefaultAsync(cancellationToken) ?? throw new ChartNotFoundException();
    }

    public async Task<IEnumerable<Chart>> GetChartsForSong(Name songName, CancellationToken cancellationToken = default)
    {
        var nameString = (string)songName;
        return await (from s in _database.Song
                join c in _database.Chart on s.Id equals c.SongId
                where s.Name == nameString
                select new Chart(c.Id, new Song(s.Name, new Uri(s.ImagePath)), Enum.Parse<ChartType>(c.Type), c.Level))
            .ToArrayAsync(cancellationToken);
    }


    public async Task<IEnumerable<Chart>> GetCoOpCharts(CancellationToken cancellationToken = default)
    {
        return await (from c in _database.Chart
                join s in _database.Song on c.SongId equals s.Id
                where c.Type == ChartType.CoOp.ToString()
                select new Chart(c.Id, new Song(s.Name, new Uri(s.ImagePath)), Enum.Parse<ChartType>(c.Type), c.Level))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IEnumerable<ChartVideoInformation>> GetChartVideoInformation(
        IEnumerable<Guid>? chartIds = default, CancellationToken cancellationToken = default)
    {
        IQueryable<ChartVideoEntity> query = _database.ChartVideo;
        if (chartIds != null)
        {
            var chartIdArray = chartIds.ToArray();
            query = query.Where(c => chartIdArray.Contains(c.ChartId));
        }

        return await query.Select(c => new ChartVideoInformation(c.ChartId, new Uri(c.VideoUrl), c.ChannelName))
            .ToArrayAsync(cancellationToken);
    }
}