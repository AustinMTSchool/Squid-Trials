using VRC.SDKBase;

public static class PersistenceUtilities
{
    public static T GetPlayerObjectComponent<T>(int playerId)
    {
        VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerId);
        if (!Utilities.IsValid(player)) return default;
        
        return GetPlayerObjectComponent<T>(player);
    }
    
    public static T GetPlayerObjectComponent<T>(VRCPlayerApi player)
    {
        if (!Utilities.IsValid(player)) return default;
        
        var objects = Networking.GetPlayerObjects(player);
        for (int i = 0; i < objects.Length; i++)
        {
            var component = objects[i].GetComponentInChildren<T>();
            if (Utilities.IsValid(component))
            {
                return component;
            }
        }
        return default;
    }
}
