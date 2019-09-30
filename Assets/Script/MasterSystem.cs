using UnityEngine;

using System.Collections;



using System.Net;

using System.Net.Sockets;

using System.Text;

using System.Threading;

using System.Collections.Generic;

public class MasterSystem : MonoBehaviour
{



    string strIP = "127.0.0.1"; // 서버 컴퓨터의 ip를 입력 하시면 됩니다.

    int port = 3800;

    Socket socket_;

    IPAddress ip;

    IPEndPoint endPoint;

    public int Server_Connent_num = 0; // 들어온 번호

    int Game_connet_Maxnum = 0; // 총 들어와 있는 유저 수

    byte[] rBuffer = new byte[1024];



    EndPoint local;



    //쓰래드

    //ThreadStart th;

    Thread serverCheck_Thread;

    bool ServerCheck_bool = true;

    bool move_bool = false;

    bool Gest_bool = false;



    //



    // 오브젝트

    public GameObject gest_object;

    public GameObject player_object;

    public List<GameObject> gest_save = new List<GameObject>();

    //



    // 정보저장

    int two_num = 0;

    Vector3 temp_move_point;





    void Start()
    {

        gest_save = new List<GameObject>();

        GameObject player = GameObject.FindWithTag("player");

        socket_ = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        ip = IPAddress.Parse(strIP);

        endPoint = new IPEndPoint(ip, port);

        socket_.Connect(endPoint);

        local = new IPEndPoint(IPAddress.Any, port);

        serverCheck_Thread = new Thread(Server_check);

        ServerFirstConnent();



    }



    // Update is called once per frame

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))

        {

            Gest_connent();

        }

        if (move_bool == true)

        {

            Object_Move(two_num, temp_move_point);

            move_bool = false;

        }

        if (Gest_bool == true)

        {

            Gest_connent();

            Gest_bool = false;

        }



    }



    void ServerFirstConnent() // 클라이언트를 실행 시키면 서버로 접속했다고 요청을 날림

    {

        string send_clientAddr = "0_0";

        Server_Send(send_clientAddr);



        rBuffer = new byte[1024];

        socket_.Receive(rBuffer, 0, rBuffer.Length, SocketFlags.None);

        string temp = Encoding.UTF8.GetString(rBuffer);

        Server_Connent_num = System.Convert.ToInt32(temp);

        Debug.Log(Server_Connent_num);

        First_connent();

        serverCheck_Thread.Start();

    }

    public void Server_Send(string buffer)

    {

        byte[] sBuffer = Encoding.UTF8.GetBytes(buffer);

        socket_.Send(sBuffer, 0, sBuffer.Length, SocketFlags.None);

    }



    void Server_check()

    {

        while (ServerCheck_bool == true)

        {

            int temp = socket_.Receive(rBuffer, 0, rBuffer.Length, SocketFlags.None);

            Debug.Log(temp);

            if (temp != -1)

            {



                Debug.Log("서버에서의 통신 vlaue : " + Encoding.UTF8.GetString(rBuffer));

                string temp_s = Encoding.UTF8.GetString(rBuffer);

                string[] temp_spritS = temp_s.Split('_');

                float[] temp_f = new float[temp_spritS.Length];

                for (int i = 0; i < temp_spritS.Length; i++)

                {

                    Debug.Log("for문 도는중" + temp_spritS[i] + " / " + temp_spritS.Length);

                    temp_f[i] = System.Convert.ToSingle(temp_spritS[i]);

                }

                Debug.Log("for문 끝");

                if (temp_f[0] == 0)

                {

                    if (temp_f[1] == 1)

                    {

                        Gest_bool = true;

                    }

                }

                else if (temp_f[0] == 1)

                {

                    Vector3 temp_v = new Vector3(temp_f[2], temp_f[3], temp_f[4]);

                    Debug.Log("vecter : " + temp_v);

                    int temp_int = (int)temp_f[1];

                    Debug.Log("int : " + temp_int);

                    two_num = temp_int;

                    temp_move_point = temp_v;

                    move_bool = true;

                }

                Debug.Log("끝");

            }



        }

    }

    void First_connent() // 첫 접속 서버에 몇명이 접속 해 있는지 확인 후 접속 인원 만큼 플레이어 오브젝트 생성

    {

        for (int i = 0; i < Server_Connent_num; i++)

        {

            if (i < Server_Connent_num - 1)

            {

                GameObject ob = Instantiate(gest_object, gest_object.transform.position, gest_object.transform.rotation) as GameObject;

                gest_save.Add(ob);

                gest_save[i].GetComponentInChildren<TextMesh>().text = (i + 1).ToString();

            }

        }

        GameObject player_ob = Instantiate(player_object, gest_object.transform.position, gest_object.transform.rotation) as GameObject;

        gest_save.Add(player_ob);

        gest_save[gest_save.Count - 1].GetComponentInChildren<TextMesh>().text = Server_Connent_num.ToString();



    }

    void Gest_connent() // 게스트가 참가 했을 경우

    {

        GameObject ob = Instantiate(gest_object, gest_object.transform.position, gest_object.transform.rotation) as GameObject;

        gest_save.Add(ob);

        gest_save[gest_save.Count - 1].GetComponentInChildren<TextMesh>().text = gest_save.Count.ToString();

    }

    void Object_Move(int List_num, Vector3 Move_Point)

    {

        Debug.Log("object_move");

        gest_save[List_num - 1].GetComponent<Object_Move>().Move_Point = Move_Point;

    }



    void OnDisable()

    {

        ServerCheck_bool = false;

        serverCheck_Thread.Abort();

        socket_.Close();

        Debug.Log("스레드 종료");

    }

}