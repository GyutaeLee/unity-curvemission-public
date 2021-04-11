using System.Collections.Generic;
using UnityEngine;

public class ShopCsvReader : MonoBehaviour
{
    private static ShopCsvReader _instance = null;
    public static ShopCsvReader instance
    {
        get
        {
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public class ShopCarItemCommonInformation
    {
        public ECarItemType eCarItemType;

        public int carItemInfoID;
        public int carItemPrice;
    }

    public class ShopCarItemCarInformation : ShopCarItemCommonInformation
    {
    }

    public class ShopCarItemPaintInformation : ShopCarItemCommonInformation
    {
    }

    public class ShopCarItemPartsInformation : ShopCarItemCommonInformation
    {
    }

    //////////////////
    // end of class //
    //////////////////

    private List<ShopCarItemCarInformation> itemCarInfo;
    private List<ShopCarItemPaintInformation> itemPaintInfo;
    private List<ShopCarItemPartsInformation> itemPartsInfo;

    private void Awake()
    {
        InitInstance();
    }

    private void Start()
    {
        ReadCsvData();
    }

    private void InitInstance()
    {
        if (ShopCsvReader.instance == null)
        {
            ShopCsvReader.instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void ReadCsvData()
    {
        ReadCsvItemCarInfo();
        ReadCsvItemPaintInfo();
        ReadCsvItemPartsInfo();
    }

    private void ReadCsvItemCarInfo()
    {
        CMCsvReader csvReader = new CMCsvReader("security-related");
        csvReader.ReadCsvFile();

        this.itemCarInfo = new List<ShopCarItemCarInformation>();

        for (int i = 0; i < csvReader.GetCsvData().Data.Count; i++)
        {
            ShopCarItemCarInformation info = new ShopCarItemCarInformation();

            info.eCarItemType = ECarItemType.Car;
            info.carItemInfoID = csvReader.GetCsvInfoID(i);
            info.carItemPrice = csvReader.GetCsvIntData(info.carItemInfoID, "security-related");

            this.itemCarInfo.Add(info);
        }
    }

    private void ReadCsvItemPaintInfo()
    {
        CMCsvReader csvReader = new CMCsvReader("security-related");
        csvReader.ReadCsvFile();

        this.itemPaintInfo = new List<ShopCarItemPaintInformation>();

        for (int i = 0; i < csvReader.GetCsvData().Data.Count; i++)
        {
            ShopCarItemPaintInformation info = new ShopCarItemPaintInformation();

            info.eCarItemType = ECarItemType.Paint;
            info.carItemInfoID = 1001 + i;
            info.carItemPrice = csvReader.GetCsvIntData(info.carItemInfoID, "security-related");

            this.itemPaintInfo.Add(info);
        }
    }

    private void ReadCsvItemPartsInfo()
    {
        CMCsvReader csvReader = new CMCsvReader("security-related");
        csvReader.ReadCsvFile();

        this.itemPartsInfo = new List<ShopCarItemPartsInformation>();

        for (int i = 0; i < csvReader.GetCsvData().Data.Count; i++)
        {
            ShopCarItemPartsInformation info = new ShopCarItemPartsInformation();

            info.eCarItemType = ECarItemType.Parts;
            info.carItemInfoID = 1001 + i;
            info.carItemPrice = csvReader.GetCsvIntData(info.carItemInfoID, "security-related");

            this.itemPartsInfo.Add(info);
        }
    }

    public ShopCarItemCommonInformation GetItemInfo(ECarItemType eCarItemType, int index)
    {
        ShopCarItemCommonInformation commonInfo;

        switch (eCarItemType)
        {
            case ECarItemType.Car:
                commonInfo = instance.itemCarInfo[index];
                break;
            case ECarItemType.Paint:
                commonInfo = instance.itemPaintInfo[index];
                break;
            case ECarItemType.Parts:
                commonInfo = instance.itemPartsInfo[index];
                break;
            default:
                commonInfo = null;
                break;
        }

        return commonInfo;
    }
}
