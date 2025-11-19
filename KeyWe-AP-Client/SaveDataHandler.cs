using System;
using System.Collections.Generic;
using System.IO;
using Global;
using HarmonyLib;
using KeyWe.Stats;
using Newtonsoft.Json;
using UnityEngine;

namespace KeyWe_AP_Client;

public class CustomSaveData
{
    public int ItemIndex = 1;

    public Dictionary<int, bool> WeeksUnlocked { get; set; } = new()
    {
        { 0, false }, { 1, false }, { 2, false },
        { 3, false }, { 4, false }, { 5, false },
        { 6, false }, { 7, false }, { 8, false }
    };

    public bool OvertimeSummerUnlocked { get; set; } = false;
    public bool OvertimeFallUnlocked { get; set; } = false;
    public bool OvertimeWinterUnlocked { get; set; } = false;
    public bool TournamentUnlocked { get; set; } = false;

    public Dictionary<int, bool> Collectibles { get; set; } = new()
    {
        { 0, false }, { 1, false }, { 2, false }, { 3, false }, { 4, false },
        { 5, false }, { 6, false }, { 7, false }, { 8, false }, { 9, false },
        { 10, false }, { 11, false }, { 12, false }, { 13, false }, { 14, false }
    };

    public Dictionary<int, bool> LevelCompletions { get; set; } = new()
    {
        { 0, false }, { 1, false }, { 2, false }, { 3, false },
        { 4, false }, { 5, false }, { 6, false }, { 7, false },
        { 8, false }, { 9, false }, { 10, false }, { 11, false },
        { 12, false }, { 13, false }, { 14, false }, { 15, false },
        { 16, false }, { 17, false }, { 18, false }, { 19, false },
        { 20, false }, { 21, false }, { 22, false }, { 23, false },
        { 24, false }, { 25, false }, { 26, false }, { 27, false },
        { 28, false }, { 29, false }, { 30, false }, { 31, false },
        { 32, false }, { 33, false }, { 34, false }, { 35, false }
    };

    public Dictionary<int, bool> OvertimeLevelCompletions { get; set; } = new()
    {
        { 0, false }, { 1, false }, { 2, false },
        { 3, false }, { 4, false }, { 5, false },
        { 6, false }, { 7, false }, { 8, false }
    };

    public Dictionary<int, bool> TournamentCourseCompletions { get; set; } = new()
    {
        { 0, false }, { 1, false }, { 2, false }
    };

    public Dictionary<int, bool> CollectiblesChecked { get; set; } = new()
    {
        { 0, false }, { 1, false }, { 2, false }, { 3, false }, { 4, false },
        { 5, false }, { 6, false }, { 7, false }, { 8, false }, { 9, false },
        { 10, false }, { 11, false }, { 12, false }, { 13, false }, { 14, false }
    };
    
    public float WalkSpeed = 10f;
    public float DashForce = 20;
    public float SwimSpeed = 5;
    public float JumpHeight = 22;
    public float RespawnFallMoveSpeed = 5;
    public float RespawnFallSpeed = 4;
    public float ChirpCooldown = 0.6f;
    public float PeckCooldown = 0.6f;
    public float DashCooldown = 0.6f;
}

public class SaveDataHandler : MonoBehaviour
{
    private static DataKeeper dataKeeper;
    private static string _seed;
    private static string _slot;
    public static CustomSaveData ArchipelagoSaveData;

    private void Start()
    {
        dataKeeper = SystemHandler.Get<DataKeeper>();
    }

    public void LoadProfile(string seed, string slot)
    {
        _seed = seed;
        _slot = slot;
        var id = (ushort)(ulong.Parse(seed.Substring(1, 15)) % ushort.MaxValue);
        dataKeeper.Initialize();
        var profile = dataKeeper.LoadProfile(id);
        if (profile == null)
        {
            ArchipelagoSaveData = new CustomSaveData();
            profile = dataKeeper.CreateProfile(id, SystemHandler.Get<StatTrackingSystem>().GetNewPlaythroughID());
            profile.MarkProgress(Enums.ProgressFlag.PlayedTutorial);
            profile.MarkProgress(Enums.ProgressFlag.WatchedBlizzardIntro);
            profile.MarkProgress(Enums.ProgressFlag.WatchedCollectionIntro);
            profile.MarkProgress(Enums.ProgressFlag.WatchedFallIntro);
            profile.MarkProgress(Enums.ProgressFlag.WatchedGameIntro);
            profile.MarkProgress(Enums.ProgressFlag.WatchedFallShiftReveal);
            profile.MarkProgress(Enums.ProgressFlag.WatchedHollyJostleIntro);
            profile.MarkProgress(Enums.ProgressFlag.WatchedWinterShiftReveal);
            profile.MarkProgress(Enums.ProgressFlag.WatchedShiftBoardIntro);
            profile.MarkProgress(Enums.ProgressFlag.WatchedWinterIntro);
            profile.MarkProgress(Enums.ProgressFlag.WatchedWickertideIntro);
            profile.CurrentLevelIndex = 35;
        }

        dataKeeper.SaveProfile(profile);
        dataKeeper.HasSelectedSaveProfile = true;
        dataKeeper.SelectProfile(profile);
        var profileSelect = FindObjectOfType<OvermapSwitchProfile>();
        profileSelect.RefreshScene();
    }

    public void UnlockWeek(int weekIndex)
    {
        ArchipelagoSaveData.WeeksUnlocked[weekIndex] = true;
    }

    [HarmonyPatch(typeof(SaveSystem))]
    private class DataRootPatch
    {
        [HarmonyPatch("DataRoot", MethodType.Getter)]
        [HarmonyPrefix]
        private static bool OnGetDataRoot(ref string __result)
        {
            var path = Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, "ArchipelagoSaves/");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            __result = path;
            return false;
        }


        [HarmonyPatch("LoadProfile")]
        [HarmonyPrefix]
        public static bool LoadProfile(ref ProfileData __result, int index)
        {
            var profileData = (ProfileData)null;
            var path = $"{SaveSystem.DataRoot}APProfile{_seed}{_slot}.kiwi";
            if (File.Exists(path))
            {
                var text = File.ReadAllText($"{SaveSystem.DataRoot}APSave{_seed}{_slot}.json");
                ArchipelagoSaveData = JsonConvert.DeserializeObject<CustomSaveData>(text);
                try
                {
                    using (var binaryReader = new BinaryReader(File.Open(path, FileMode.Open)))
                    {
                        profileData = JsonUtility.FromJson<ProfileData>(StringEncryption.Decrypt(
                            binaryReader.ReadString(), "TallKikiIsCursed_967f2d41-a0a0-42fa-a7ed-79b2c5977569"));
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[KEYWE][SaveSystem] LoadData - Exception thrown while loading data:\n{ex}");
                }
            }

            __result = profileData;
            return false;
        }

        [HarmonyPatch("SaveProfile")]
        [HarmonyPrefix]
        public static bool SaveProfile(ProfileData profile, string profileName = "Profile")
        {
            var json = JsonConvert.SerializeObject(ArchipelagoSaveData);
            File.WriteAllText($"{SaveSystem.DataRoot}APSave{_seed}{_slot}.json", json);
            SaveSystem.TryCreateFolderDemo();
            var path = $"{SaveSystem.DataRoot}APProfile{_seed}{_slot}.kiwi";
            if (profile == null)
                return false;
            try
            {
                using (var binaryWriter = new BinaryWriter(File.Open(path, FileMode.Create)))
                {
                    var str = StringEncryption.Encrypt(JsonUtility.ToJson(profile),
                        "TallKikiIsCursed_967f2d41-a0a0-42fa-a7ed-79b2c5977569");
                    binaryWriter.Write(str);
                }

                SaveSystem.BackupSave(profile);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            return false;
        }
    }
}