using UnityEngine;
using UnityEngine.UI;

// TO DO : TEST CODE
public class TEST_Garage : MonoBehaviour
{
    public Text TXT_carInfoID;
    public Text TXT_carPaintID;

    private void Start()
    {
        int carInfoID = SecurityPlayerPrefs.GetInt("security-related", InventoryInformation.GetDefaultCarInfoID());
        int carPaintID = SecurityPlayerPrefs.GetInt("security-related", InventoryInformation.GetDefaultPaintInfoID());

        TXT_carInfoID.text = "CAR INFO ID  : " + carInfoID;
        TXT_carPaintID.text = "CAR PAINT ID  : " + carPaintID;
    }

    public void ChangecarInfoID()
    {
        int carInfoID = SecurityPlayerPrefs.GetInt("security-related", InventoryInformation.GetDefaultCarInfoID());

        carInfoID++;

        if (carInfoID <= 1000)
        {
            carInfoID = 1001;
        }
        else if (carInfoID > 1004)
        {
            carInfoID = 1001;
        }
        else if (carInfoID == 1002)
        {
            carInfoID = 1003;
        }

        SecurityPlayerPrefs.SetInt("security-related", carInfoID);

        TXT_carInfoID.text = "CAR INFO ID  : " + carInfoID;
    }

    public void ChangecarPaintID()
    {
        int carPaintID = SecurityPlayerPrefs.GetInt("security-related", InventoryInformation.GetDefaultPaintInfoID());

        carPaintID++;

        if (carPaintID < 1000)
        {
            carPaintID = 1001;
        }
        else if (carPaintID > 1002)
        {
            carPaintID = 1001;
        }

        SecurityPlayerPrefs.SetInt("security-related", carPaintID);

        TXT_carPaintID.text = "CAR PAINT ID  : " + carPaintID;
    }
}
