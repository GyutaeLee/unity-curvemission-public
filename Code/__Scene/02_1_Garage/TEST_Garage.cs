using UnityEngine;
using UnityEngine.UI;

// TO DO : TEST CODE
public class TEST_Garage : MonoBehaviour
{
    public Text TXT_carInfoID;
    public Text TXT_carPaintID;

    private void Start()
    {
        int carInfoID = SecurityPlayerPrefs.GetInt("security-related", 0);
        int carPaintID = SecurityPlayerPrefs.GetInt("security-related", 0);

        TXT_carInfoID.text = "CAR INFO ID  : " + carInfoID;
        TXT_carPaintID.text = "CAR PAINT ID  : " + carPaintID;
    }

    public void ChangecarInfoID()
    {
        int carInfoID = SecurityPlayerPrefs.GetInt("security-related", 0);

        carInfoID++;

        if (carInfoID <= 0)
        {
            carInfoID = 0;
        }
        else if (carInfoID > 0)
        {
            carInfoID = 0;
        }
        else if (carInfoID == 0)
        {
            carInfoID = 0;
        }

        SecurityPlayerPrefs.SetInt("security-related", carInfoID);

        TXT_carInfoID.text = "CAR INFO ID  : " + carInfoID;
    }

    public void ChangecarPaintID()
    {
        int carPaintID = SecurityPlayerPrefs.GetInt("security-related", 0);

        carPaintID++;

        if (carPaintID < 0)
        {
            carPaintID = 0;
        }
        else if (carPaintID > 0)
        {
            carPaintID = 0;
        }

        SecurityPlayerPrefs.SetInt("security-related", carPaintID);

        TXT_carPaintID.text = "CAR PAINT ID  : " + carPaintID;
    }
}
