﻿using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ScoreTracker.Application.Queries;
using ScoreTracker.Data.Persistence;
using ScoreTracker.Data.Persistence.Entities;
using ScoreTracker.Domain.Enums;
using ScoreTracker.Domain.Models;
using ScoreTracker.Domain.Records;
using ScoreTracker.Domain.SecondaryPorts;

namespace ScoreTracker.Data.Repositories
{
    public sealed class EFTournamentRepository : ITournamentRepository,
        IRequestHandler<GetTournamentRolesQuery, IEnumerable<UserTournamentRole>>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ChartAttemptDbContext _database;
        private readonly IChartRepository _charts;
        private readonly IDbContextFactory<ChartAttemptDbContext> _factory;

        public EFTournamentRepository(IMemoryCache memoryCache, IChartRepository charts,
            IDbContextFactory<ChartAttemptDbContext> factory)
        {
            _factory = factory;
            _database = factory.CreateDbContext();
            _memoryCache = memoryCache;
            _charts = charts;
        }

        private static string TourneyCacheKey = $@"{nameof(EFTournamentRepository)}_Tournies";
        private static string TourneyIdCacheKey(Guid id) => $@"{nameof(EFTournamentRepository)}_Tourney_{id}";


        public async Task<IEnumerable<TournamentRecord>> GetAllTournaments(CancellationToken cancellationToken)
        {
            return await _memoryCache.GetOrCreateAsync(TourneyCacheKey, async o =>
            {
                o.AbsoluteExpiration = DateTimeOffset.Now + TimeSpan.FromMinutes(60);
                var counts = (await _database.UserTournamentSession.ToArrayAsync(cancellationToken))
                    .GroupBy(uts => uts.TournamentId)
                    .ToDictionary(g => g.Key, g => g.Count());

                return (await _database.Tournament.ToArrayAsync(cancellationToken)).Select(t =>
                    new TournamentRecord(t.Id,
                        t.Name,
                        counts.TryGetValue(t.Id, out var count) ? count : 0,
                        Enum.Parse<TournamentType>(t.Type),
                        t.Location, t.IsHighlighted,
                        Uri.TryCreate(t.LinkOverride, UriKind.Absolute, out var url) ? (Uri?)url : null,
                        t.StartDate,
                        t.EndDate,
                        t.IsMoM)).ToArray();
            });
        }

        public async Task<TournamentConfiguration> GetTournament(Guid id, CancellationToken cancellationToken)
        {
            return await _memoryCache.GetOrCreateAsync(TourneyIdCacheKey(id), async o =>
            {
                o.AbsoluteExpiration = DateTimeOffset.Now + TimeSpan.FromMinutes(60);
                var result = await _database.Tournament.Where(t => t.Id == id).SingleAsync(cancellationToken);
                var snapshots = await GetScoringLevelSnapshot(id, cancellationToken);
                return JsonSerializer.Deserialize<TournamentConfigurationJsonEntity>(result.Configuration)
                           ?.To(snapshots) ??
                       throw new Exception($"Tournament {id} was not configured properly");
            });
        }

        public async Task CreateOrSaveTournament(TournamentConfiguration tournament,
            CancellationToken cancellationToken)
        {
            var entity = await _database.Tournament.FirstOrDefaultAsync(t => t.Id == tournament.Id, cancellationToken);
            if (entity == null)
            {
                await _database.Tournament.AddAsync(new TournamentEntity
                {
                    Id = tournament.Id,
                    Configuration = JsonSerializer.Serialize(TournamentConfigurationJsonEntity.From(tournament)),
                    EndDate = tournament.EndDate,
                    StartDate = tournament.StartDate,
                    IsHighlighted = tournament.IsHighlighted,
                    IsMoM = tournament.IsMom,
                    Name = tournament.Name
                }, cancellationToken);
            }
            else
            {
                entity.Name = tournament.Name;
                entity.EndDate = tournament.EndDate;
                entity.IsHighlighted = tournament.IsHighlighted;
                entity.StartDate = tournament.StartDate;
                entity.IsMoM = tournament.IsMom;
                entity.Configuration = JsonSerializer.Serialize(TournamentConfigurationJsonEntity.From(tournament));
            }

            await _database.SaveChangesAsync(cancellationToken);
            _memoryCache.Remove(TourneyCacheKey);
            _memoryCache.Remove(TourneyIdCacheKey(tournament.Id));
        }

        public async Task CreateOrSaveTournament(TournamentRecord tournament, CancellationToken cancellationToken)
        {
            var database = await _factory.CreateDbContextAsync(cancellationToken);
            var entity = await database.Tournament.Where(t => t.Id == tournament.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
            {
                await database.Tournament.AddAsync(new TournamentEntity
                {
                    Configuration = "{}",
                    EndDate = tournament.EndDate,
                    Id = tournament.Id,
                    IsHighlighted = tournament.IsHighlighted,
                    IsMoM = tournament.IsMoM,
                    LinkOverride = tournament.LinkOverride?.ToString(),
                    Location = tournament.Location,
                    Name = tournament.Name,
                    StartDate = tournament.StartDate,
                    Type = tournament.Type.ToString()
                }, cancellationToken);
            }
            else
            {
                entity.EndDate = tournament.EndDate;
                entity.Id = tournament.Id;
                entity.IsHighlighted = tournament.IsHighlighted;
                entity.IsMoM = tournament.IsMoM;
                entity.LinkOverride = tournament.LinkOverride?.ToString();
                entity.Location = tournament.Location;
                entity.Name = tournament.Name;
                entity.StartDate = tournament.StartDate;
                entity.Type = tournament.Type.ToString();
                entity.IsMoM = tournament.IsMoM;
            }

            await _database.SaveChangesAsync(cancellationToken);
            _memoryCache.Remove(TourneyCacheKey);
            _memoryCache.Remove(TourneyIdCacheKey(tournament.Id));
        }

        public async Task SaveSession(TournamentSession session, CancellationToken cancellationToken)
        {
            _memoryCache.Remove(TourneyCacheKey);

            var entity = await _database.UserTournamentSession.FirstOrDefaultAsync(
                uts => uts.TournamentId == session.TournamentId && uts.UserId == session.UsersId, cancellationToken);
            if (entity == null)
            {
                await _database.UserTournamentSession.AddAsync(new UserTournamentSessionEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = session.UsersId,
                    TournamentId = session.TournamentId,
                    SessionScore = session.TotalScore,
                    RestTime = session.CurrentRestTime,
                    ChartsPlayed = session.Entries.Count(),
                    AverageDifficulty = session.Entries.Average(e => e.Chart.Level),
                    NeedsApproval = session.NeedsApproval,
                    VideoUrl = session.VideoUrl?.ToString(),
                    VerificationType = session.VerificationType.ToString(),
                    ChartEntries = JsonSerializer.Serialize(session.Entries.Select(e => new SessionEntryEntity
                    {
                        ChartId = e.Chart.Id,
                        IsBroken = e.IsBroken,
                        Plate = e.Plate.ToString(),
                        Score = e.Score,
                        SessionScore = e.SessionScore,
                        BonusPoints = e.BonusPoints
                    }))
                }, cancellationToken);
            }
            else
            {
                if (!session.Entries.Any())
                {
                    _database.UserTournamentSession.Remove(entity);
                }
                else
                {
                    entity.SessionScore = session.TotalScore;
                    entity.RestTime = session.CurrentRestTime;
                    entity.NeedsApproval = session.NeedsApproval;
                    entity.VerificationType = session.VerificationType.ToString();
                    entity.ChartsPlayed = session.Entries.Count();
                    entity.VideoUrl = session.VideoUrl?.ToString();
                    entity.AverageDifficulty = session.Entries.Average(e => e.Chart.Level);
                    entity.ChartEntries = JsonSerializer.Serialize(session.Entries.Select(e => new SessionEntryEntity
                    {
                        ChartId = e.Chart.Id,
                        IsBroken = e.IsBroken,
                        Plate = e.Plate.ToString(),
                        Score = e.Score,
                        SessionScore = e.SessionScore,
                        BonusPoints = e.BonusPoints
                    }));
                }
            }

            var existingPhotos = await _database.PhotoVerification
                .Where(p => p.TournamentId == session.TournamentId && p.UserId == session.UsersId)
                .ToArrayAsync(cancellationToken);

            var newUrls = session.PhotoUrls.Select(u => u.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var toDelete = existingPhotos.Where(p => !newUrls.Contains(p.PhotoUrl));
            var toCreate = newUrls.Where(u =>
                    !existingPhotos.Any(p => p.PhotoUrl.Equals(u, StringComparison.OrdinalIgnoreCase)))
                .Select(p => new PhotoVerificationEntity
                {
                    Id = Guid.NewGuid(),
                    PhotoUrl = p,
                    TournamentId = session.TournamentId,
                    UserId = session.UsersId
                });
            _database.PhotoVerification.RemoveRange(toDelete);
            await _database.PhotoVerification.AddRangeAsync(toCreate, cancellationToken);
            await _database.SaveChangesAsync(cancellationToken);
            _memoryCache.Remove(CacheKey(session.TournamentId, session.UsersId));
        }

        private string CacheKey(Guid tournamentId, Guid userId)
        {
            return $"{nameof(EFTournamentRepository)}__Tournament:{tournamentId}__User:{userId}";
        }

        public async Task<TournamentSession> GetSession(Guid tournamentId, Guid userId,
            CancellationToken cancellationToken)
        {
            return await _memoryCache.GetOrCreateAsync(CacheKey(tournamentId, userId), async o =>
            {
                o.AbsoluteExpiration = DateTimeOffset.Now + TimeSpan.FromDays(1);

                var entity = await _database.UserTournamentSession.FirstOrDefaultAsync(
                    uts => uts.TournamentId == tournamentId && uts.UserId == userId, cancellationToken);
                var tournamentConfig = await GetTournament(tournamentId, cancellationToken);
                if (entity == null) return new TournamentSession(userId, tournamentConfig);

                var entryEntities = JsonSerializer.Deserialize<SessionEntryEntity[]>(entity.ChartEntries) ??
                                    Array.Empty<SessionEntryEntity>();
                var charts = (await _charts.GetCharts(MixEnum.Phoenix,
                    chartIds: entryEntities.Select(e => e.ChartId).Distinct().ToArray(),
                    cancellationToken: cancellationToken)).ToDictionary(c => c.Id);
                var entries = entryEntities.Select(e => new TournamentSession.Entry(charts[e.ChartId], e.Score,
                    Enum.Parse<PhoenixPlate>(e.Plate), e.IsBroken, e.SessionScore, e.BonusPoints ?? 0));

                var session = new TournamentSession(userId, tournamentConfig);
                foreach (var entry in entries)
                    session.AddWithoutApproval(entry.Chart, entry.Score, entry.Plate, entry.IsBroken);
                session.SetVerificationType(Enum.Parse<SubmissionVerificationType>(entity.VerificationType));
                if (Uri.TryCreate(entity.VideoUrl, UriKind.Absolute, out var videoUrl)) session.VideoUrl = videoUrl;

                var photos = await _database.PhotoVerification
                    .Where(p => p.TournamentId == tournamentId && p.UserId == userId)
                    .ToArrayAsync(cancellationToken);
                foreach (var photo in photos.Select(p => p.PhotoUrl))
                {
                    if (!Uri.TryCreate(photo, UriKind.Absolute, out var photoUrl)) continue;

                    session.PhotoUrls.Add(photoUrl);
                }

                if (!entity.NeedsApproval) session.Approve();

                return session;
            });
        }

        public async Task<IEnumerable<LeaderboardRecord>> GetLeaderboardRecords(Guid tournamentId,
            CancellationToken cancellationToken)
        {
            var results = (await (from uts in _database.UserTournamentSession
                        join u in _database.User on uts.UserId equals u.Id
                        where uts.TournamentId == tournamentId
                        select new UserEntryDto(u.Id, u.Name, uts.SessionScore, uts.RestTime, uts.ChartsPlayed,
                            uts.AverageDifficulty, uts.VerificationType, uts.NeedsApproval, uts.VideoUrl))
                    .ToArrayAsync(cancellationToken))
                .OrderByDescending(ue => ue.Score)
                .Select((ue, index) => new LeaderboardRecord(index + 1, ue.UserId, ue.Name, ue.Score, ue.RestTime,
                    ue.AverageDifficulty, ue.ChartsPlayed, Enum.Parse<SubmissionVerificationType>(ue.VerificationType),
                    ue.NeedsApproval,
                    ue.VideoUrl == null ? null :
                    Uri.TryCreate(ue.VideoUrl, UriKind.Absolute, out var vidUrl) ? vidUrl : null)).ToArray();
            foreach (var result in results)
                result.Session = await GetSession(tournamentId, result.UserId, cancellationToken);

            return results;
        }

        public async Task CreateScoringLevelSnapshots(Guid tournamentId, IEnumerable<(Guid, double)> snapshots,
            CancellationToken cancellationToken)
        {
            var database = await _factory.CreateDbContextAsync(cancellationToken);
            database.TournamentChartLevel.RemoveRange(
                await database.TournamentChartLevel.Where(l => l.TournamentId == tournamentId)
                    .ToArrayAsync(cancellationToken));

            await database.TournamentChartLevel.AddRangeAsync(snapshots.Select(s => new TournamentChartLevelEntity
            {
                Id = Guid.NewGuid(),
                ChartId = s.Item1,
                Level = s.Item2,
                TournamentId = tournamentId
            }), cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
        }

        private string SnapshotCacheKey(Guid tournamentId)
        {
            return $"{nameof(EFTournamentRepository)}__{tournamentId}__LevelSnapshot";
        }

        public async Task<IDictionary<Guid, double>?> GetScoringLevelSnapshot(Guid tournamentId,
            CancellationToken cancellationToken)
        {
            return await _memoryCache.GetOrCreateAsync(SnapshotCacheKey(tournamentId), async o =>
            {
                o.AbsoluteExpiration = DateTimeOffset.Now + TimeSpan.FromHours(1);
                var result = await _database.TournamentChartLevel.Where(e => e.TournamentId == tournamentId)
                    .ToArrayAsync(cancellationToken);
                if (!result.Any()) return (IDictionary<Guid, double>?)null;

                return result.ToDictionary(e => e.ChartId, e => e.Level);
            });
        }

        public async Task SetRole(Guid tournamentId, Guid userId, TournamentRole role,
            CancellationToken cancellationToken)
        {
            var entity =
                await _database.TournamentRole.FirstOrDefaultAsync(
                    e => e.TournamentId == tournamentId && e.UserId == userId, cancellationToken);
            if (entity == null)
                await _database.TournamentRole.AddAsync(new TournamentRoleEntity
                {
                    Role = role.ToString(),
                    TournamentId = tournamentId,
                    UserId = userId
                }, cancellationToken);
            else
                entity.Role = role.ToString();
            await _database.SaveChangesAsync(cancellationToken);
            _memoryCache.Remove(TourneyRolesCacheKey(tournamentId));
        }

        public async Task RevokeRole(Guid tournamentId, Guid userId, CancellationToken cancellationToken)
        {
            var removingEntities = await
                _database.TournamentRole.Where(e => e.TournamentId == tournamentId && e.UserId == userId)
                    .ToArrayAsync(cancellationToken);
            _database.TournamentRole.RemoveRange(removingEntities);
            await _database.SaveChangesAsync(cancellationToken);
            _memoryCache.Remove(TourneyRolesCacheKey(tournamentId));
        }


        private sealed record UserEntryDto(Guid UserId, string Name, int Score, TimeSpan RestTime, int ChartsPlayed,
            double AverageDifficulty, string VerificationType, bool NeedsApproval, string? VideoUrl)
        {
        }

        private static string TourneyRolesCacheKey(Guid tournamentId)
        {
            return $@"{nameof(EFTournamentRepository)}_TourneyRoles_{tournamentId}";
        }

        public async Task<IEnumerable<UserTournamentRole>> Handle(GetTournamentRolesQuery request,
            CancellationToken cancellationToken)
        {
            return await _memoryCache.GetOrCreateAsync(TourneyRolesCacheKey(request.TournamentId), async o =>
            {
                o.AbsoluteExpiration = DateTime.Now + TimeSpan.FromHours(1);
                return _database.TournamentRole.Where(t => t.TournamentId == request.TournamentId)
                    .Select(t => new UserTournamentRole(t.TournamentId, t.UserId, Enum.Parse<TournamentRole>(t.Role)));
            });
        }
    }
}