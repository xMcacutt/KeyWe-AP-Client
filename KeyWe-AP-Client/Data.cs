using System.Collections.Generic;

namespace KeyWe_AP_Client;

public class Data
{
    public static float MaxWalkSpeed = 16;
    public static float MaxJumpHeight = 33;
    public static float MaxSwimSpeed = 12.4f;
    public static float MaxRespawnFallSpeed = 10;
    public static float MaxRespawnFallMoveSpeed = 13;
    public static float MaxDashForce = 37.5f;
    public static float MaxChirpCooldown = 5;
    public static float MinDashCooldown = 0.3f;
    public static float MinPeckCooldown = 0.0f;
    public static float MinChirpCooldown = 0.0f;
    public static float InitialWalkSpeed = 10f;
    public static float InitialDashForce = 20;
    public static float InitialSwimSpeed = 5;
    public static float InitialJumpHeight = 22;
    public static float InitialRespawnFallMoveSpeed = 5;
    public static float InitialRespawnFallSpeed = 4;
    public static float InitialChirpCooldown = 0.6f;
    public static float InitialPeckCooldown = 0.6f;
    public static float InitialDashCooldown = 0.6f;


    public static Dictionary<string, int> OvertimeLevelNameToId = new()
    {
        { "Kiwis in Harmony", 0 },
        { "Conveyer Belt Chaos", 1 },
        { "Lunch Break", 2 },
        { "Tank Trouble", 3 },
        { "Cassowary Courier Course", 4 },
        { "Bubble Wrap Testing", 5 },
        { "Snowball Fight!", 6 },
        { "The Sorting Room", 7 },
        { "Cashing Out", 8 }
    };

    public static Dictionary<string, int> TournamentLevelNameToId = new()
    {
        { "Gumtree Grove", 0 },
        { "Painted Cliffs", 1 },
        { "Lake Bessy", 2 }
    };

    public static List<int> ExcludedLevels = [16, 18, 19, 17];
    public static List<int> ExcludedOvertimeShifts = [3];
    public static List<int> ExcludedTournamentCourses = [1, 2];

    public static Dictionary<string, int> LevelNameToId = new()
    {
        { "The Telegraph Desk", 0 },
        { "The Transcription Room", 1 },
        { "The Shipping Floor", 2 },
        { "The Dropoff Depot", 3 },
        { "Marauding Mailflies", 4 },
        { "Covert Decoders", 5 },
        { "Postal Pest Problems", 6 },
        { "Vegetation Vexation", 7 },
        { "Devilish Dust-Up", 8 },
        { "A Sinking Feeling", 9 },
        { "Bouncing Boxes (and Blimps)", 10 },
        { "Creepin’ Kudzu", 11 },
        { "Shipping Shake-Up", 12 },
        { "Transcription Turmoil", 13 },
        { "Keyboard Commotion", 14 },
        { "Parcel Panel Puzzle", 15 },
        { "The Night Post", 16 },
        { "Electrical Interference", 17 },
        { "Bobbing for Boxes", 18 },
        { "Mechanical Mayhem", 19 },
        { "Tricks and Telegrams", 20 },
        { "Zoey’s Tracks of Terror", 21 },
        { "Casso-scary", 22 },
        { "Mail from Beyond", 23 },
        { "Trapdoors and Tentacles", 24 },
        { "Assembly-Line Scramble", 25 },
        { "Dueling Crates", 26 },
        { "Switchboard Synchrony", 27 },
        { "Bungalow Basin Bake-Off", 28 },
        { "Parts and Crafts", 29 },
        { "The Hollyjostle Tinkertrack", 30 },
        { "That’s a Wrap", 31 },
        { "An Approaching Storm", 32 },
        { "Bitter Cold", 33 },
        { "Emergency Relief", 34 },
        { "Stand Your Post", 35 }
    };
}