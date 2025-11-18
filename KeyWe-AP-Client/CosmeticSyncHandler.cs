using ExitGames.Client.Photon;
using Global.Online;
using Photon.Pun;
using Photon.Realtime;

public class CosmeticSyncHandler : MonoBehaviourPunCallbacks
{
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer)
            return;
        if (!changedProps.TryGetValue<int[]>(Properties.Customization, out var itemIDs))
            return;
        ApplyRemoteCosmetics(itemIDs);
    }

    private void ApplyRemoteCosmetics(int[] itemIDs)
    {
        var playerKiwis = FindObjectsOfType<Kiwi>();
        foreach (var kiwi in playerKiwis)
        {
            if (kiwi.IsLocalPlayer)
                continue;
            kiwi.Customization.Init(itemIDs);
        }
    }
}