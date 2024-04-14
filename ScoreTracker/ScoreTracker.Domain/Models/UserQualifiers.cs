﻿using ScoreTracker.Domain.Records;
using ScoreTracker.Domain.ValueTypes;

namespace ScoreTracker.Domain.Models
{
    public sealed class UserQualifiers
    {
        public QualifiersConfiguration Configuration { get; }

        public bool IsApproved { get; private set; }
        public Name UserName { get; set; }

        public IDictionary<Guid, Submission> Submissions { get; }

        public IEnumerable<(Chart, PhoenixScore)> BestCharts()
        {
            return Configuration.Charts.Where(c => Submissions.ContainsKey(c.Id)).OrderByDescending(c => Rating(c.Id))
                .Select(c => (c, Submissions[c.Id].Score))
                .Take(Configuration.PlayCount).ToArray();
        }

        private readonly ScoringConfiguration _scoreConfig = new()
        {
            ContinuousLetterGradeScale = true
        };

        public double Rating(DifficultyLevel level, PhoenixScore score)
        {
            if (Configuration.ScoringType == "Fungpapi") return level + (score - 965000.0) / 17500.0;
            return _scoreConfig.GetScore(level, score);
        }

        public double Rating(Guid chartId)
        {
            if (!Submissions.ContainsKey(chartId))
            {
                return 0;
            }

            var difficulty = Configuration.Charts.First(c => c.Id == chartId).Level;
            return Rating(difficulty, Submissions[chartId].Score);
        }

        public double CalculateScore()
        {
            return BestCharts().Sum(c => Rating(c.Item1.Id));
        }

        public void Approve()
        {
            IsApproved = true;
        }

        public UserQualifiers(QualifiersConfiguration config, bool isApproved, Name userName,
            IDictionary<Guid, Submission> submissions)
        {
            Configuration = config;
            IsApproved = isApproved;
            UserName = userName;
            Submissions = submissions;
        }

        public bool AddXXScore(Guid chartId, StepCount perfects, StepCount greats, StepCount goods, StepCount bads,
            StepCount misses, StepCount maxCombo,
            Uri uri)
        {
            var offset = Configuration.NoteCountAdjustments.TryGetValue(chartId, out var adjustment)
                ? adjustment
                : 0;
            perfects += offset;
            maxCombo += offset;
            var scoreScreen = new ScoreScreen(perfects, greats, goods, bads, misses, maxCombo);
            return AddPhoenixScore(chartId, scoreScreen.CalculatePhoenixScore, uri);
        }

        public bool AddPhoenixScore(Guid chartId, PhoenixScore score, Uri uri)
        {
            if (Submissions.ContainsKey(chartId) && Submissions[chartId].Score > score) return false;

            Submissions[chartId] = new Submission
            {
                ChartId = chartId,
                PhotoUrl = uri,
                Score = score
            };
            IsApproved = false;
            return true;
        }

        public sealed class Submission
        {
            public Guid ChartId { get; set; }
            public PhoenixScore Score { get; set; }
            public Uri PhotoUrl { get; set; }
        }
    }
}
