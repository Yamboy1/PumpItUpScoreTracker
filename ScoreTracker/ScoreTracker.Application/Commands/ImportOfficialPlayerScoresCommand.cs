﻿using MediatR;

namespace ScoreTracker.Application.Commands
{
    public sealed record ImportOfficialPlayerScoresCommand
    (string Username, string Password, string Id, string ExpectedGameTag, bool IncludeBroken,
        bool SyncPiuTracker) : IRequest
    {
    }
}
