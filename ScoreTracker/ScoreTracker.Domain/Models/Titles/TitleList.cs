﻿using System.Collections.Immutable;
using ScoreTracker.Domain.Enums;

namespace ScoreTracker.Domain.Models.Titles;

public static class TitleList
{
    private static readonly Title[] Titles =
    {
        new BasicTitle("Beginner", "Default title"),
        new StepArtistTitle("EXC"),
        new StepArtistTitle("NIMGO"),
        new StepArtistTitle("WINDFORCE"),
        new StepArtistTitle("BME"),
        new StepArtistTitle("CONRAD"),
        new StepArtistTitle("OSING"),
        new StepArtistTitle("FEFEMZ"),
        new StepArtistTitle("AEVILUX"),
        new StepArtistTitle("SPHAM"),
        new StepArtistTitle("SUNNY"),
        new ArtistTitle("MAX"),
        new ArtistTitle("ATAS"),
        new ArtistTitle("SHK"),
        new ArtistTitle("NATO"),
        new ArtistTitle("DOIN"),
        new ArtistTitle("PORY"),
        new ArtistTitle("KIEN"),
        new ArtistTitle("HYUN"),
        new ArtistTitle("QUREE"),
        new ArtistTitle("APPLESODA"),
        new StepArtistTitle("DM Ashura"),
        new DifficultyLevelTitle("Intermediate LV.1", 10, 11, 25),
        new DifficultyLevelTitle("Intermediate LV.2", 10, 11, 50),
        new DifficultyLevelTitle("Intermediate LV.3", 12, 13, 25),
        new DifficultyLevelTitle("Intermediate LV.4", 12, 13, 50),
        new DifficultyLevelTitle("Intermediate LV.5", 14, 15, 25),
        new DifficultyLevelTitle("Intermediate LV.6", 14, 15, 50),
        new DifficultyLevelTitle("Intermediate LV.7", 16, 17, 25),
        new DifficultyLevelTitle("Intermediate LV.8", 16, 17, 50),
        new DifficultyLevelTitle("Intermediate LV.9", 18, 19, 25),
        new DifficultyLevelTitle("Intermediate LV.10", 18, 19, 50),
        new DifficultyLevelTitle("Advanced LV.1", 20, 25),
        new DifficultyLevelTitle("Advanced LV.2", 20, 50),
        new DifficultyLevelTitle("Advanced LV.3", 21, 25),
        new DifficultyLevelTitle("Advanced LV.4", 21, 50),
        new DifficultyLevelTitle("Advanced LV.5", 22, 20),
        new DifficultyLevelTitle("Advanced LV.6", 22, 40),
        new DifficultyLevelTitle("Advanced LV.7", 22, 60),
        new DifficultyLevelTitle("Advanced LV.8", 23, 20),
        new DifficultyLevelTitle("Advanced LV.9", 23, 35),
        new DifficultyLevelTitle("Advanced LV.10", 23, 50),
        new DifficultyLevelTitle("Expert LV.1", 24, 30, "Requires Advanced LV.10. All Random Trains count once."),
        new DifficultyLevelTitle("Expert LV.2", 25, 15, "Requires Expert LV.1."),
        new DifficultyLevelTitle("Expert LV.3", 26, 7, "Requires Expert LV.2."),
        new DifficultyLevelTitle("Expert LV.4", 27, 3, "Requires Expert LV.3."),
        new DifficultyLevelTitle("The Master", 28, 1, "Requires Expert LV.4."),
        new SkillTitle("Twist LV.1", "Street Show Down", ChartType.Single, 15),
        new SkillTitle("Twist LV.2", "Final Audition 3", ChartType.Single, 16),
        new SkillTitle("Twist LV.3", "U Got Me Rocking", ChartType.Single, 17),
        new SkillTitle("Twist LV.4", "Final Audition", ChartType.Double, 18),
        new SkillTitle("Twist LV.5", "Super Fantasy", ChartType.Single, 19),
        new SkillTitle("Twist LV.6", "Witch Doctor #1", ChartType.Double, 20),
        new SkillTitle("Twist LV.7", "Love is a Danger Zone", ChartType.Double, 21),
        new SkillTitle("Twist LV.8", "Love is a Danger Zone", ChartType.Single, 22),
        new SkillTitle("Twist LV.9", "Love is a Danger Zone (Cranky Mix)", ChartType.Double, 23),
        new SkillTitle("Twist LV.10", "Bee", ChartType.Double, 24),
        new BasicTitle("Twist Expert", "Get all 10 Twist titles", "Skill"),
        new SkillTitle("Run LV.1", "Final Audition", ChartType.Double, 15),
        new SkillTitle("Run LV.2", "Super Fantasy", ChartType.Single, 16),
        new SkillTitle("Run LV.3", "Pavane", ChartType.Single, 17),
        new SkillTitle("Run LV.4", "Gothique Resonance", ChartType.Single, 18),
        new SkillTitle("Run LV.5", "Napalm", ChartType.Single, 19),
        new SkillTitle("Run LV.6", "Bee", ChartType.Double, 20),
        new SkillTitle("Run LV.7", "Sarabande", ChartType.Double, 21),
        new SkillTitle("Run LV.8", "Just Hold On (To All Fighters)", ChartType.Double, 22),
        new SkillTitle("Run LV.9", "Final Audition EP.2-X", ChartType.Single, 23, XXLetterGrade.S),
        new SkillTitle("Run LV.10", "Yog Sothoth", ChartType.Double, 24),
        new BasicTitle("Run Expert", "Get all 10 Run titles", "Skill"),
        new SkillTitle("Drill LV.1", "Vook", ChartType.Single, 15),
        new SkillTitle("Drill LV.2", "Solitary 1.5", ChartType.Single, 16),
        new SkillTitle("Drill LV.3", "Gun Rock", ChartType.Single, 17),
        new SkillTitle("Drill LV.4", "Moonlight", ChartType.Single, 18),
        new SkillTitle("Drill LV.5", "Vacuum", ChartType.Single, 19),
        new SkillTitle("Drill LV.6", "Overblow", ChartType.Single, 20),
        new SkillTitle("Drill LV.7", "Sorceress Elise", ChartType.Single, 21),
        new SkillTitle("Drill LV.8", "Rock the House", ChartType.Double, 22),
        new SkillTitle("Drill LV.9", "Witch Doctor", ChartType.Double, 23),
        new SkillTitle("Drill LV.10", "Wi-Ex-Doc-Va", ChartType.Double, 24),
        new BasicTitle("Drill Expert", "Get all 10 Drill titles", "Skill"),
        new SkillTitle("Gimmick LV.1", "Yeo Rae A", ChartType.Single, 13),
        new SkillTitle("Gimmick LV.2", "Bad Apple", ChartType.Single, 15),
        new SkillTitle("Gimmick LV.3", "Love Scenario", ChartType.Single, 17),
        new SkillTitle("Gimmick LV.4", "Come to Me", ChartType.Single, 17),
        new SkillTitle("Gimmick LV.5", "Rock the House (Short Cut)", ChartType.Single, 18),
        new SkillTitle("Gimmick LV.6", "Miss S' Story", ChartType.Single, 19),
        new SkillTitle("Gimmick LV.7", "Nakakapagpabagabag", ChartType.Single, 19),
        new SkillTitle("Gimmick LV.8", "Twist of Fate", ChartType.Single, 19),
        new SkillTitle("Gimmick LV.9", "Everybody Got 2 Know", ChartType.Single, 19),
        new SkillTitle("Gimmick LV.10", "86", ChartType.Single, 20),
        new BasicTitle("Gimmick Expert", "Get all 10 Gimmick Titles", "Skill"),
        new SkillTitle("Half LV.1", "Trashy Innocence", ChartType.Double, 16),
        new SkillTitle("Half LV.2", "Butterfly", ChartType.Double, 17),
        new SkillTitle("Half LV.3", "Shub Niggurath", ChartType.Double, 18),
        new SkillTitle("Half LV.4", "Super Fantsy", ChartType.Double, 18),
        new SkillTitle("Half LV.5", "Phantom", ChartType.Double, 19),
        new SkillTitle("Half LV.6", "Utsushiyo no Kaze", ChartType.Double, 20),
        new SkillTitle("Half LV.7", "Witch Doctor #1", ChartType.Double, 21),
        new SkillTitle("Half LV.8", "Bad Apple (Full Song)", ChartType.Double, 22),
        new SkillTitle("Half LV.9", "Love is a Danger Zone (Try to B.P.M)", ChartType.Double, 23),
        new SkillTitle("Half LV.10", "Imprinting", ChartType.Double, 24),
        new BasicTitle("Half Expert", "Get all 10 Half titles", "Skill"),
        new BasicTitle("Specialist", "Get all 50 skill titles", "Skill"),
        new CustomTitle("Co-Op Beginner", "5 S on CoOp", 5, "CoOp",
            c => c.Chart.Type == ChartType.CoOp &&
                 c.BestAttempt?.LetterGrade is XXLetterGrade.S or XXLetterGrade.SS or XXLetterGrade.SSS),
        new CustomTitle("Co-Op Intermediate", "20 S/SS (not SSS) on CoOp", 20, "CoOp",
            c => c.Chart.Type == ChartType.CoOp && c.BestAttempt?.LetterGrade is XXLetterGrade.S or XXLetterGrade.SS),
        new CustomTitle("Co-Op Advanced", "30 S/SS (not SSS) on CoOp", 30, "CoOp",
            c => (c.Chart.Type == ChartType.CoOp) & c.BestAttempt?.LetterGrade is XXLetterGrade.S or XXLetterGrade.SS),
        new CustomTitle("Co-Op Expert", "40 SSS on CoOp", 40, "CoOp",
            c => c.Chart.Type == ChartType.CoOp && c.BestAttempt?.LetterGrade == XXLetterGrade.SSS),
        new CustomTitle("Co-Op Master", "55 SSS on CoOp", 55, "CoOp",
            c => c.Chart.Type == ChartType.CoOp && c.BestAttempt?.LetterGrade == XXLetterGrade.SSS),
        new BasicTitle("Macnom", "Stage fail on 2nd song in a credit 10 times"),
        new BasicTitle("Gold Member", "100 Play Count", "Play Count"),
        new BasicTitle("Platinum Member", "500 Play Count", "Play Count"),
        new BasicTitle("Diamond Member", "1000 Play Count", "Play Count"),
        new BasicTitle("VIP Member", "5000 Play Count", "Play Count"),
        new BasicTitle("VVIP Member", "10000 Play Count", "Play Count"),
        new BasicTitle("Super Man", "15 stage pass S in a row", "Letter Grades"),
        new BasicTitle("SSuper Man", "15 stage pass SS in a row", "Letter Grades"),
        new BasicTitle("SSSuper Man", "15 stage pass SSS in a row", "Letter Grades"),
        new BasicTitle("Ace Player", "15 stage pass A in a row", "Letter Grades"),
        new BasicTitle("Best Player", "5 stage pass B in a row", "Letter Grades"),
        new BasicTitle("Capable Player", "5 stage pass C i na row", "Letter Grades"),
        new BasicTitle("Dazzling Player", "5 stage pass D in a row", "Letter Grades"),
        new BasicTitle("Fantastic Player", "2 stage pass F in a row", "Letter Grades"),
        new BasicTitle("Great Player", "10000 Greats accumulated", "Timing"),
        new BasicTitle("Good Player", "10000 Goods accumulated", "Timing"),
        new BasicTitle("Bad Master", "10000 Bads accumulated", "Timing"),
        new BasicTitle("B.P.M Follower", "Get exactly 1 great, 0 goods, 2 misses on Beethoven virus D18"),
        new BasicTitle("Pump is a Sense", "S with no bads on Love is a Danger Zone D21"),
        new BasicTitle("No Skills No Pump", "SS on Moonlight D21"),
        new BasicTitle("Cheater", "S on D26, additional unknown condition"),
        new BasicTitle("Scrooge", "200,000 PP accumulated at once"),
        new BasicTitle("Lovers", "Play any CO-OP chart 100 times (including repeat plays)", "CoOp"),
        new BasicTitle("Prime 2 Specialist (green)", "Prime 2 Total Rank score 101-1000", "Event"),
        new BasicTitle("Prime 2 Specialist (blue)", "Prime 2 Total Rank score 11-999", "Event"),
        new BasicTitle("Prime 2Specialist (red)", "Prime 2 Total Rank score 1-10"),
        new BasicTitle("Prime 2 VIP (green)", "Prime 2 EXP 101-1000", "Event"),
        new BasicTitle("Prime 2 VIP (blue)", "Prime 2 EXP 11-99", "Event"),
        new BasicTitle("Prime 2 VIP (red)", "Prime 2 EXP 1-10", "Event"),
        new BasicTitle("???", "Korean location test attendee", "Event"),
        new BasicTitle("2019 PSF 1st Place", "Pump Summer Festival 1st Place", "Event"),
        new BasicTitle("2019 PSF Top 10", "Pump Summer Festival 2nd-10th place", "Event"),
        new BasicTitle("2019 PSF Top 30", "Pump Summer Festival 11th-30th place", "Event"),
        new BasicTitle("2019 PSF Top 50", "Pump Summer Festival 31st-50th place", "Event"),
        new BasicTitle("2020 PWF 1st Place", "2020 Pump Winter Festival 1st Place", "Event"),
        new BasicTitle("2020 PWF Top 10", "2020 Pump Winter Festival 2nd-10th place", "Event"),
        new BasicTitle("2020 PWF Top 30", "2020 Pump Winter Festival 11th-30th place", "Event"),
        new BasicTitle("2020 PWF Top 50", "2020 Pump Winter Festival 31st-50th place", "Event"),
        new BasicTitle("2020 PWF Top 100", "2020 Pump Winter Festival 51st-100th place", "Event"),
        new BasicTitle("Avatar Collector LV.1", "Get 5 avatars, not counting default", "Collector"),
        new BasicTitle("Avatar Collector LV.2", "Get 10 avatars, not counting default", "Collector"),
        new BasicTitle("Avatar Collector LV.3", "Get 25 avatars, not counting default", "Collector"),
        new BasicTitle("Avatar Collector LV.4", "Get 50 avatars, not counting default", "Collector"),
        new BasicTitle("Avatar Collector LV.5", "Get 80 avatars, not counting default", "Collector"),
        new BasicTitle("Title Collector LV.1", "Get 5 titles, not counting Beginner", "Collector"),
        new BasicTitle("Title Collector LV.2", "Get 10 titles, not counting Beginner", "Collector"),
        new BasicTitle("Title Collector LV.3", "Get 30 titles, not counting Beginner", "Collector"),
        new BasicTitle("Title Collector LV.4", "Get 50 titles, not counting Beginner", "Collector"),
        new BasicTitle("Title Collector LV.5", "Get 100 titles, not counting Beginner", "Collector"),
        new BasicTitle("Ultimate Collector", "Get all Avatar Collector and Title Collector titles", "Collector"),
        new BasicTitle("OoooooooooooAAAAEAAIAU", "Play Brain Power 10 Times", "Songs"),
        new BasicTitle("Wandering Pianist", "Play Life is Piano 10 times", "Songs"),
        new BasicTitle("God Mode", "Play God Mode 2.0 10 times", "Songs"),
        new BasicTitle("Doomsday Survivor", "Play Harmagedon 10 times", "Songs"),
        new BasicTitle("Crossing Delta", "Play Crossing Delta 10 times", "Songs"),
        new BasicTitle("Star-Crossed", "Play Repentance 10 times", "Songs"),
        new BasicTitle("Glory of Revenge", "Play Gloria 10 times", "Songs"),
        new BasicTitle("Tap Dancer", "Play Papasito FULL SONG 5 times", "Songs")
    };

    public static IEnumerable<TitleProgress> BuildProgress(IEnumerable<BestXXChartAttempt> attempts)
    {
        var progress = Titles.Select(t => new TitleProgress(t)).ToImmutableArray();
        foreach (var attempt in attempts)
        foreach (var title in progress)
            title.ApplyAttempt(attempt);

        return progress;
    }
}