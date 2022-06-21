using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
public class jsontest : MonoBehaviour {

    //public class Account
    //{
    //    public string Email { get; set; }
    //    public bool Active { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public IList<string> Roles { get; set; }
    //    public Vector3 Ve { get; set; }
    //    public Dictionary<string, Vector3> StrVector3Dictionary { get; set; }
    //}


    //private void Awake()
    //{
    //    var accountJames = new Account
    //    {
    //        Email = "james@example.com",
    //        Active = true,
    //        CreatedDate = new DateTime(2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
    //        Roles = new List<string>
    //        {
    //            "User",
    //            "Admin"
    //        },
    //        Ve = new Vector3(10, 3, 1),
    //        StrVector3Dictionary = new Dictionary<string, Vector3>
    //        {
    //            {"start", new Vector3(0, 0, 1)},
    //            {"end", new Vector3(9, 0, 1)}
    //        }
    //    };


    //    var accountOnion = new Account
    //    {
    //        Email = "onion@example.co.uk",
    //        Active = true,
    //        CreatedDate = new DateTime(2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
    //        Roles = new List<string>
    //        {
    //            "User",
    //            "Admin"
    //        },
    //        Ve = new Vector3(0, 3, 1),
    //        StrVector3Dictionary = new Dictionary<string, Vector3>
    //        {
    //            {"vr", new Vector3(0, 0, 1)},
    //            {"pc", new Vector3(9, 9, 1)}
    //        }
    //    };


    //    var setting = new JsonSerializerSettings();
    //    setting.Formatting = Formatting.Indented;
    //    setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

    //    // write
    //    var accountsFromCode = new List<Account> { accountJames, accountOnion };
    //    var json = JsonConvert.SerializeObject(accountsFromCode, setting);
    //    var path = Path.Combine(Application.dataPath, "hi.json");
    //    File.WriteAllText(path, json);

    //    // read
    //    var fileContent = File.ReadAllText(path);
    //    var accountsFromFile = JsonConvert.DeserializeObject<List<Account>>(fileContent);
    //    var reSerializedJson = JsonConvert.SerializeObject(accountsFromFile, setting);

    //    print(reSerializedJson);
    //    print("json == reSerializedJson is" + (json == reSerializedJson));
    //}

    //class Packet
    //{
    //    public enum KIND
    //    {
    //        LOG_IN,
    //        CHATTING,
    //        LOBBY,
    //        GAME
    //    };
    //    public KIND kind;
    //    public string message;
    //    public List<int> itemList = new List<int>();
    //}

    //class Data
    //{
    //    public byte[] _SendArray;
    //    public Data(JObject JData)
    //    {
    //        MemoryStream ms = new MemoryStream();
    //        using (BsonWriter writer = new BsonWriter(ms))
    //        {
    //            JsonSerializer serializer = new JsonSerializer();
    //            serializer.Serialize(writer, JData);
    //            _SendArray = ms.ToArray();
    //        }

    //        MemoryStream ms2 = new MemoryStream(_SendArray);

    //        JObject JData2;
    //        using (BsonReader reader = new BsonReader(ms2))
    //        {
    //            JData2 = (JObject)JToken.ReadFrom(reader);
    //        }

    //    }
    //}

    // Use this for initialization


    void Start () {

        CWJSon jData = new CWJSon();
        JObject jj = new JObject();

        JArray ja = new JArray();

        for(int i=0;i<100;i++)
        {
            ja.Add(i.ToString());
        }
        


        jj.Add("array", ja);
        jj.Add("a1", 1);
        jj.Add("a2", 2);
        jj.Add("a3", 3);

        jData.Add("jj",jj);


        byte[] pData = jData.ToArray();

        CWJSon jData2 = new CWJSon();

        jData2.SetData(pData);

        JToken jt= jData2.GetJson("jj");

        int t1= jt["a1"].Value<int>();
        int t2 = jt["a2"].Value<int>();
        int t3 = jt["a3"].Value<int>();

        JArray ja1 = (JArray)jt["array"];

        int tcnt = ja1.Count;

        for(int i=0;i<tcnt;i++)
        {
            print(ja1[i]);
        }

        foreach(var v in ja1)
        {
            print(v.Value<string>());
        }
        
        JObject j = (JObject)jt;

        foreach(JProperty v in j.Properties())
        {
            print(string.Format("{0} : {1}",v.Name,v.Value));
        }
        

     

        //JToken jt = jData.GetJson("JJ");


        //JObject jj = new JObject();
        //jj.Add("a", 1);
        //jj.Add("b", 2);
        //jj.Add("c", 3);
        //Data  dt= new Data(jj);



        //MemoryStream ms = new MemoryStream(_ReceiveArray.GetRange(0, _Size - 4).ToArray());


        /*
                Packet p = new Packet(); 
                p.kind = Packet.KIND.CHATTING;
                p.message = "Hello";
                p.itemList.Add(123);
                p.itemList.Add(124);
                p.itemList.Add(125);

                // Packet클래스를 BSON으로 직렬화
                MemoryStream ms = new MemoryStream();
                JsonSerializer serializer = new JsonSerializer();
                BsonWriter writer = new BsonWriter(ms);
                serializer.Serialize(writer, p);
                ms.Seek(0, SeekOrigin.Begin);

                // BSON을 바이트로 전환해 화면에 출력
                byte[] byteBSON = ms.ToArray();
                int byteLength = 0;

                foreach (byte a in byteBSON)
                {
                    byteLength++;
                    print(a + " ");
                }
                print("byte 길이 : " + byteLength);


                BsonReader reader = new BsonReader(ms);
                Packet dp = serializer.Deserialize<Packet>(reader);

                print("패킷의 종류 : " + dp.kind);
                print("패킷 내용 : " + dp.message);
                print("itemList의 길이 : " + dp.itemList.Count);
                */




    }

    // Update is called once per frame
    void Update () {
		
	}
}
