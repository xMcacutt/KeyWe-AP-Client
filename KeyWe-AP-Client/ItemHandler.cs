using System;
using Archipelago.MultiClient.Net.Models;
using Global;
using UnityEngine;

namespace KeyWe_AP_Client;

public enum KWItem
{
    SummerWeek1 = 1,
    SummerWeek2 = 2,
    SummerWeek3 = 3,
    FallWeek1 = 4,
    FallWeek2 = 5,
    FallWeek3 = 6,
    WinterWeek1 = 7,
    WinterWeek2 = 8,
    WinterWeek3 = 9,
    OvertimeSummer = 0xA,
    OvertimeFall = 0xB,
    OvertimeWinter = 0xC,
    Tournament = 0xD,
    Facewear = 0x100,
    Hat = 0x101,
    Skin = 0x102,
    Backwear = 0x103,
    Hairstyle = 0x104,
    Footwear = 0x105,
    Arms = 0x106,
    DashUp = 0x200,
    MoveUp = 0x201,
    SwimUp = 0x202,
    JumpUp = 0x203,
    RespawnUp = 0x204,
    ChirpUp = 0x205,
    PeckUp =  0x206,
    SecretSpiceShaker = 0x300,
    GlimmeringShell = 0x301,
    EmptyChrysalis = 0x302,
    TemperedLens = 0x303,
    WayfarersCompass = 0x304,
    PricklySeedPod = 0x305,
    AncientTooth = 0x306,
    CosmicFriendshipRock = 0x307,
    ChargedFeather = 0x308,
    SaltyScale = 0x309,
    GlowingWishbone = 0x30A,
    Z39SoaringAuk = 0x30B,
    PapaMoonFigurine = 0x30C,
    MountaineersPiton = 0x30D,
    LostLetter = 0x30E
}

public class ItemHandler
{
    public void HandleItem(int index, ItemInfo item)
    {
        try
        {
            if (index < SaveDataHandler.ArchipelagoSaveData.ItemIndex)
                return;
            SaveDataHandler.ArchipelagoSaveData.ItemIndex++;
            switch (item.ItemId)
            {
                case > 0x0 and < 0xA:
                    PluginMain.SaveDataHandler.UnlockWeek((int)item.ItemId - 1);
                    break;
                case (int)KWItem.OvertimeSummer:
                    SaveDataHandler.ArchipelagoSaveData.OvertimeSummerUnlocked = true;
                    break;
                case (int)KWItem.OvertimeFall:
                    SaveDataHandler.ArchipelagoSaveData.OvertimeFallUnlocked = true;
                    break;
                case (int)KWItem.OvertimeWinter:
                    SaveDataHandler.ArchipelagoSaveData.OvertimeWinterUnlocked = true;
                    break;
                case (int)KWItem.Tournament:
                    if (SystemHandler.Get<DataKeeper>().Profile.CheckProgress(Enums.ProgressFlag.TournamentDLC))
                        SaveDataHandler.ArchipelagoSaveData.TournamentUnlocked = true;
                    else
                        APConsole.Instance.Log(
                            "Warning: You do not own the Telepost Tournament DLC but have its checks enabled.");
                    break;
                case (int)KWItem.Facewear:
                    PluginMain.GameHandler.EquipRandom(Customizables.Categories.Face);
                    break;
                case (int)KWItem.Hat:
                    PluginMain.GameHandler.EquipRandom(Customizables.Categories.Hat);
                    break;
                case (int)KWItem.Skin:
                    PluginMain.GameHandler.EquipRandom(Customizables.Categories.Skin);
                    break;
                case (int)KWItem.Backwear:
                    PluginMain.GameHandler.EquipRandom(Customizables.Categories.Back);
                    break;
                case (int)KWItem.Hairstyle:
                    PluginMain.GameHandler.EquipRandom(Customizables.Categories.Hair);
                    break;
                case (int)KWItem.Footwear:
                    PluginMain.GameHandler.EquipRandom(Customizables.Categories.Footwear);
                    break;
                case (int)KWItem.Arms:
                    PluginMain.GameHandler.EquipRandom(Customizables.Categories.Arms);
                    break;
                case (int)KWItem.DashUp:
                    var currentDashCooldown = SaveDataHandler.ArchipelagoSaveData.DashCooldown;
                    var dashIncrement = (Data.InitialDashCooldown - Data.MinDashCooldown) / 5;
                    SaveDataHandler.ArchipelagoSaveData.DashCooldown =
                        Math.Max(currentDashCooldown - dashIncrement, Data.MinDashCooldown);
                    var currentDashForce = SaveDataHandler.ArchipelagoSaveData.DashForce;
                    dashIncrement = (Data.MaxDashForce - Data.InitialDashForce) / 5;
                    SaveDataHandler.ArchipelagoSaveData.DashForce =
                        Math.Min(currentDashForce + dashIncrement, Data.MaxDashForce);
                    GameHandler.onReceivedMovementUpgrade?.Invoke();
                    break;
                case (int)KWItem.MoveUp:
                    var currentWalkSpeed = SaveDataHandler.ArchipelagoSaveData.WalkSpeed;
                    var walkIncrement = (Data.MaxWalkSpeed - Data.InitialWalkSpeed) / 5;
                    SaveDataHandler.ArchipelagoSaveData.WalkSpeed =
                        Math.Min(currentWalkSpeed + walkIncrement, Data.MaxWalkSpeed);
                    GameHandler.onReceivedMovementUpgrade?.Invoke();
                    break;
                case (int)KWItem.SwimUp:
                    var currentSwimSpeed = SaveDataHandler.ArchipelagoSaveData.SwimSpeed;
                    var swimIncrement = (Data.MaxSwimSpeed - Data.InitialSwimSpeed) / 5;
                    SaveDataHandler.ArchipelagoSaveData.SwimSpeed =
                        Math.Min(currentSwimSpeed + swimIncrement, Data.MaxSwimSpeed);
                    GameHandler.onReceivedMovementUpgrade?.Invoke();
                    break;
                case (int)KWItem.JumpUp:
                    var currentJumpHeight = SaveDataHandler.ArchipelagoSaveData.JumpHeight;
                    var jumpIncrement = (Data.MaxJumpHeight - Data.InitialJumpHeight) / 5;
                    SaveDataHandler.ArchipelagoSaveData.JumpHeight =
                        Math.Min(currentJumpHeight + jumpIncrement, Data.MaxJumpHeight);
                    GameHandler.onReceivedMovementUpgrade?.Invoke();
                    break;
                case (int)KWItem.RespawnUp:
                    var currentRespawnFallSpeed = SaveDataHandler.ArchipelagoSaveData.RespawnFallSpeed;
                    var respawnFallSpeedIncrement = (Data.MaxRespawnFallSpeed - Data.InitialRespawnFallSpeed) / 5;
                    SaveDataHandler.ArchipelagoSaveData.RespawnFallSpeed =
                        Math.Min(currentRespawnFallSpeed + respawnFallSpeedIncrement, Data.MaxRespawnFallSpeed);
                    var currentRespawnFallMoveSpeed = SaveDataHandler.ArchipelagoSaveData.RespawnFallMoveSpeed;
                    var respawnFallMoveSpeedIncrement =
                        (Data.MaxRespawnFallMoveSpeed - Data.InitialRespawnFallMoveSpeed) / 5;
                    SaveDataHandler.ArchipelagoSaveData.RespawnFallMoveSpeed = Math.Min(
                        currentRespawnFallMoveSpeed + respawnFallMoveSpeedIncrement, Data.MaxRespawnFallMoveSpeed);
                    GameHandler.onReceivedMovementUpgrade?.Invoke();
                    break;
                case (int)KWItem.ChirpUp:
                    var currentChirpCooldown = SaveDataHandler.ArchipelagoSaveData.ChirpCooldown;
                    var chirpIncrement = (Data.InitialChirpCooldown - Data.MinChirpCooldown) / 5;
                    SaveDataHandler.ArchipelagoSaveData.ChirpCooldown = Math.Max(currentChirpCooldown - chirpIncrement,
                        Data.MinChirpCooldown);
                    GameHandler.onReceivedMovementUpgrade?.Invoke();
                    break;
                case (int)KWItem.PeckUp:
                    var currentPeckCooldown = SaveDataHandler.ArchipelagoSaveData.PeckCooldown;
                    var peckIncrement = (Data.InitialPeckCooldown - Data.MinPeckCooldown) / 5;
                    SaveDataHandler.ArchipelagoSaveData.PeckCooldown =
                        Math.Max(currentPeckCooldown - peckIncrement, Data.MinPeckCooldown);
                    GameHandler.onReceivedMovementUpgrade?.Invoke();
                    break;
                case > 0x2FF and < 0x30F:
                    SaveDataHandler.ArchipelagoSaveData.Collectibles[(int)item.ItemId - 0x300] = true;
                    break;
            }

            SystemHandler.Get<DataKeeper>().SaveProfile();
        }
        catch (Exception ex)
        {
            APConsole.Instance.Log($"[HandleItem ERROR] {ex}");
            throw;
        }
    }
}
