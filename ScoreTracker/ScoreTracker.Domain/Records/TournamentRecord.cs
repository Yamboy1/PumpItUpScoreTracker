﻿using ScoreTracker.Domain.Enums;
using ScoreTracker.Domain.ValueTypes;

namespace ScoreTracker.Domain.Records
{
    public sealed record TournamentRecord(Guid Id, Name Name, int CurrentParticipants, TournamentType Type,
        string Location,
        bool IsHighlighted,
        Uri? LinkOverride,
        DateTimeOffset? StartDate,
        DateTimeOffset? EndDate,
        bool IsMoM)
    {
    }
}