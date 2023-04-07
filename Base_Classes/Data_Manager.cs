using System;
using System.IO;
using System.Collections;

public class Data_Manager
{

    public string[] data;
    public ArrayList dataFields = new ArrayList();
    public int dataStartIndex = -1;

    string pastPath = "";
    string pastDataPath = "";
    string pastValue = "";
    string pastFieldIdentifier = "";
    

    public Data_Manager(string path)
    {
        loadFileData(path);
        pastPath = path;
    }

    public void reloadData(bool isToSaveEarlierData = true)
    {
        if (isToSaveEarlierData)
        {
            saveData();
        }
        loadFileData(pastPath);
        loadData(pastValue, pastFieldIdentifier);

    }

    public void loadFileData(string path)
    {
        string[] dataFieldData = File.ReadAllLines(path);

        pastDataPath = dataFieldData[0];

        data = File.ReadAllLines(pastDataPath);

        for (int i = 1; i < dataFieldData.Length; i++)
        {
            string eLineData = dataFieldData[i].Trim();
            if (eLineData.Length > 0)
            {
                dataFields.Add(eLineData);
            }

        }
    }

    public void loadData(string value, string fieldIdentifier)
    {

        pastValue = value;
        pastFieldIdentifier = fieldIdentifier;

        for (int i = 0; i < data.Length; i++)
        {
            string eLineData = data[i].Trim();
            if (eLineData.Length < 0) return;

            int checkingDataIndex = i + dataFields.IndexOf(fieldIdentifier);
            if (data[checkingDataIndex] == value)
            {
                dataStartIndex = i;
                break;
            }
            else
            {
                i += dataFields.Count;
            }
        }
    }

    public string getData(string fieldName)
    {
        return data[dataStartIndex + dataFields.IndexOf(fieldName)];
    }

    public void setData(string fieldName, string value)
    {
        data[dataStartIndex + dataFields.IndexOf(fieldName)] = value;
    }

    public void saveData()
    {
        File.WriteAllLines(pastPath, data);
    }

    public ArrayList getSpecificFieldValues(string fieldIdentifierName)
    {
        ArrayList values = new ArrayList();

        for (int i = 0; i < data.Length; i++)
        {
            string eLineData = data[i].Trim();

            if (eLineData.Length > 0)
            {
                values.Add(data[i + dataFields.IndexOf(fieldIdentifierName)]);
                i += dataFields.Count;
            }
        }


        return values;

    }

}