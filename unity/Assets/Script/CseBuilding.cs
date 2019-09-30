using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using MongoDB.Bson.Serialization.Attributes;

public class CseBuilding : MonoBehaviour
{
    public Vector3[] location = new Vector3[5];
    // Start is called before the first frame update
    public GameObject obj1;
    public GameObject obj2;
    public GameObject gateway0;
    public GameObject gateway1;
    public GameObject gateway2;
    public GameObject gateway3;
    public GameObject gateway4;
    public bool enableSpawn = false;
    public int[] rssis = new int[5];
    public string[] gate = new string[5];
    public int temp;
    SortedDictionary<int,string> mote = new SortedDictionary<int, string>();
    public int[] twovalue = new int[5];
    public string[] twovalues = new string[5];
    Dictionary<string, Vector3>[] gateway = new Dictionary<string, Vector3>[5];
   
    string connectionString = "mongodb://localhost:27017";
    void SpawnEnemy()
    {
        
        temp = 0;
        location[0] = new Vector3(9, 0, 12);
        location[1] = new Vector3(7, 0, 4);
        location[2] = new Vector3(2, 0, 15);
        location[3] = new Vector3(16, 0, 4);
        location[4] = new Vector3(14, 0, 12);

        gate[0] = "3b";
        gate[1] = "ad";
        gate[2] = "e1";
        gate[3] = "df";
        gate[4] = "43";

        /*for (int i = 0; i < 5; ++i)
        {
            gateway[i].Add(gate[i],location[i]);
        }*/
        // Debug.Log(gateway[0].Values.ToString());
        var client = new MongoClient(connectionString);
        var server = client.GetServer();
        server.Connect();
        var database = server.GetDatabase("yeonghun");
        var shopCollection1 = database.GetCollection("person1");
        var shopCollection2 = database.GetCollection("person2");
        if (shopCollection1.Count() == 1)
        {
            if (shopCollection1.FindOne().GetValue("rssi") > 60)
            {
                if (shopCollection1.FindOne().GetValue("MOTE") == "43")
                {
                    GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(location[4].x + 8, 0, location[4].z), Quaternion.identity);
                    shopCollection1.Drop();
                    Destroy(Obj1, 2);
                }
                else if (shopCollection1.FindOne().GetValue("MOTE") == "df")
                {
                    GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(location[3].x, 0, location[3].z - 3), Quaternion.identity);
                    shopCollection1.Drop();
                    Destroy(Obj1, 2);

                }
                else if (shopCollection1.FindOne().GetValue("MOTE") == "ad")
                {
                    GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(location[1].x - 4, 0, location[1].z - 3 ), Quaternion.identity);
                    shopCollection1.Drop();
                    Destroy(Obj1, 2);
                }
                else if (shopCollection1.FindOne().GetValue("MOTE") == "e1")
                {
                    GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(location[2].x, 0, location[2].z - 5), Quaternion.identity);
                    shopCollection1.Drop();
                    Destroy(Obj1, 2);
                }
            }
        }

        if (shopCollection2.Count() == 1)
        {
            if (shopCollection2.FindOne().GetValue("rssi") > 60)
            {
                if (shopCollection2.FindOne().GetValue("MOTE") == "43")
                {
                    GameObject Obj2 = (GameObject)Instantiate(obj1, new Vector3(location[4].x + 8, 0, location[4].z), Quaternion.identity);
                    shopCollection2.Drop();
                    Destroy(Obj2, 2);
                    return;
                }
                else if (shopCollection2.FindOne().GetValue("MOTE") == "df")
                {
                    GameObject Obj2 = (GameObject)Instantiate(obj1, new Vector3(location[3].x, 0, location[3].z - 3), Quaternion.identity);
                    shopCollection2.Drop();
                    Destroy(Obj2, 2);
                    return;

                }
                else if (shopCollection2.FindOne().GetValue("MOTE") == "ad")
                {
                    GameObject Obj2 = (GameObject)Instantiate(obj1, new Vector3(location[1].x - 4, 0, location[1].z - 3), Quaternion.identity);
                    shopCollection2.Drop();
                    Destroy(Obj2, 2);
                    return;
                }
                else if (shopCollection2.FindOne().GetValue("MOTE") == "e1")
                {
                    GameObject Obj2 = (GameObject)Instantiate(obj1, new Vector3(location[2].x, 0, location[2].z - 5), Quaternion.identity);
                    shopCollection2.Drop();
                    Destroy(Obj2, 2);
                    return;
                }
            }
        }
        /*if(shopCollection1.Count() > 2)
        {
            int i = 0;
            foreach (var document in shopCollection1.Find(new QueryDocument()))
            {
                mote.Add(int.Parse(document.GetValue("rssi").ToString()),document.GetValue("MOTE").ToString());
            }
            
            foreach(KeyValuePair<int, string> pair in mote)
            {
                
            }
        }*/
        /*foreach (var document in shopCollection1.Find(new QueryDocument()))
        {
            if(document.GetValue("MOTE") == "3b")
            {
                rssis[0] = int.Parse(document.GetValue("rssi").ToString());
                if (rssis[0] >= 80)
                {
                    location[0].x = 0;
                    location[0].z = 0;
                    temp++;
                }
            }
            else if (document.GetValue("MOTE") == "ad")
            {
                rssis[1] = int.Parse(document.GetValue("rssi").ToString());
                if (rssis[1] >= 80)
                {
                    location[1].x = 0;
                    location[1].z = 0;
                    temp++;
                }
            }
            else if (document.GetValue("MOTE") == "e1")
            {
                rssis[2] = int.Parse(document.GetValue("rssi").ToString());
                if (rssis[2] >= 80)
                {
                    location[2].x = 0;
                    location[2].z = 0;
                    temp++;
                }
            }
            else if (document.GetValue("MOTE") == "df")
            {
                rssis[3] = int.Parse(document.GetValue("rssi").ToString());
                if (rssis[3] >= 80)
                {
                    location[3].x = 0;
                    location[3].z = 0;
                    temp++;
                }
            }
            else if (document.GetValue("MOTE") == "43")
            {
                rssis[4] = int.Parse(document.GetValue("rssi").ToString());
                if (rssis[4] >= 80)
                {
                    location[4].x = 0;
                    location[4].z = 0;
                    temp++;
                }
            }
        }*/

        /*foreach (var document in shopCollection1.Find(new QueryDocument()))
        {
            if (int.Parse(document.GetValue("rssi").ToString()) < 60)
            {
                if (document.GetValue("MOTE") == "3b")
                {
                    GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(location[0].x, 0, location[0].z), Quaternion.identity);
                    shopCollection1.Drop();
                    Destroy(Obj1, 2);
                    return;
                }
                else if (document.GetValue("MOTE") == "ad")
                {
                    GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(location[1].x, 0, location[1].z), Quaternion.identity);
                    shopCollection1.Drop();
                    Destroy(Obj1, 2);
                    return;
                }
                else if (document.GetValue("MOTE") == "e1")
                {
                    GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(location[2].x, 0, location[2].z), Quaternion.identity);
                    shopCollection1.Drop();
                    Destroy(Obj1, 2);
                    return;
                }
                else if (document.GetValue("MOTE") == "df")
                {
                    GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(location[3].x, 0, location[3].z), Quaternion.identity);
                    shopCollection1.Drop();
                    Destroy(Obj1, 2);
                }
                else if (document.GetValue("MOTE") == "43")
                {
                    GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(location[4].x, 0, location[4].z), Quaternion.identity);
                    shopCollection1.Drop();
                    Destroy(Obj1, 1);
                    return;
                }
            }
        }*/
        int rssi1 = 100 ;
        foreach(var document in shopCollection1.Find(new QueryDocument()))
        {
            if (rssi1 > int.Parse(document.GetValue("rssi").ToString()))
                rssi1 = int.Parse(document.GetValue("rssi").ToString());
        }

        foreach (var document in shopCollection1.Find(new QueryDocument()))
        {
            if (rssi1 == int.Parse(document.GetValue("rssi").ToString()))
            {
                for(int i = 0; i<5; ++i)
                {
                    if(document.GetValue("MOTE").ToString() == gate[i])
                    {
                        GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(location[i].x, 0, location[i].z), Quaternion.identity);
                        shopCollection1.Drop();
                        Destroy(Obj1, 2);
                    }
                }
            }
        }

        int rssi2 = 100;
        foreach (var document in shopCollection2.Find(new QueryDocument()))
        {
            if (rssi2 > int.Parse(document.GetValue("rssi").ToString()))
                rssi2 = int.Parse(document.GetValue("rssi").ToString());
        }

        foreach (var document in shopCollection2.Find(new QueryDocument()))
        {
            if (rssi2 == int.Parse(document.GetValue("rssi").ToString()))
            {
                for (int i = 0; i < 5; ++i)
                {
                    if (document.GetValue("MOTE").ToString() == gate[i])
                    {
                        GameObject Obj2 = (GameObject)Instantiate(obj1, new Vector3(location[i].x, 0, location[i].z), Quaternion.identity);
                        shopCollection2.Drop();
                        Destroy(Obj2, 2);
                    }
                }
            }
        }

        Debug.Log(shopCollection1.Count());
        if (shopCollection1.Count() > 0) shopCollection1.Drop();
           
        float xpos1 = (location[0].x + location[1].x + location[2].x + location[3].x + location[4].x) / (int)(shopCollection1.Count()-temp);
        float zpos1 = (location[0].z + location[1].z + location[2].z + location[3].z + location[4].z) / (int)(shopCollection1.Count()-temp);
        Debug.Log(shopCollection1.Count()-temp);

        Debug.Log(shopCollection2.Count());
        if (shopCollection2.Count() > 0) shopCollection2.Drop();

        float xpos2 = (location[0].x + location[1].x + location[2].x + location[3].x + location[4].x) / (int)(shopCollection1.Count() - temp);
        float zpos2 = (location[0].z + location[1].z + location[2].z + location[3].z + location[4].z) / (int)(shopCollection1.Count() - temp);
        Debug.Log(shopCollection2.Count() - temp);
        /*if (enableSpawn)
        {
            if (shopCollection1.Count() != 0 && shopCollection1.Count() <= 5)
            {
                GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(xpos, 0, zpos), Quaternion.identity);
                // GameObject Obj2 = (GameObject)Instantiate(obj2, new Vector3(9, 0, 24), Quaternion.identity);
                Destroy(Obj1, 1);
                // Destroy(Obj2,5);
            }
        }*/
        // shopCollection1.Drop();

        for (int i = 0; i < 5; ++i)
            rssis[i] = 0;
        mote.Clear(); //SortedDictionary 지우는 함수
    }

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 1, 2);
        location[0] = new Vector3(9, 0, 12);
        location[1] = new Vector3(7, 0, 4);
        location[2] = new Vector3(2, 0, 15);
        location[3] = new Vector3(16, 0, 4);
        location[4] = new Vector3(14, 0, 12);
        GameObject Gateway0 = (GameObject)Instantiate(gateway0, location[0], Quaternion.identity);
        GameObject Gateway1 = (GameObject)Instantiate(gateway1, location[1], Quaternion.identity);
        GameObject Gateway2 = (GameObject)Instantiate(gateway2, location[2], Quaternion.identity);
        GameObject Gateway3 = (GameObject)Instantiate(gateway3, location[3], Quaternion.identity);
        GameObject Gateway4 = (GameObject)Instantiate(gateway4, location[4], Quaternion.identity);
        GameObject Obj1 = (GameObject)Instantiate(obj1, new Vector3(7, 0, 4), Quaternion.identity);
        GameObject Obj2 = (GameObject)Instantiate(obj2, new Vector3(16, 0, 4), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        /*obj.transform.position = new Vector3(10, 0, 1);
        Debug.Log(obj.transform.position);
        obj.transform.position = new Vector3(11, 0, 1);
        Debug.Log(obj.transform.position);
        obj.transform.position = new Vector3(12, 0, 1);
        Debug.Log(obj.transform.position);
        obj.transform.position = new Vector3(13, 0, 1);
        Debug.Log(obj.transform.position);
        obj.transform.position = new Vector3(14, 0, 1);

        Debug.Log(obj.transform.position);*/

    }
}
