using UnityEngine;

public enum EVillageType
{
    None = 0,

    Village = 1,
    Garage = 2,
    Shop = 3,
    Ranking = 4,

    Max,
}

public class VillageManager : MonoBehaviour
{
    private void Start()
    {
        InitVillageManager();
    }

    private void InitVillageManager()
    {
        BgmManager.instance.LoadBgmResources(EBgmType.Village, SecurityPlayerPrefs.GetInt("security-related", ConvertVillageTypeToIndex(EVillageType.Village)));
        BgmManager.instance.PlayGameBgm(true);
    }

    public static int ConvertVillageTypeToIndex(EVillageType eVillageType)
    {
        return (int)eVillageType;
    }
}
