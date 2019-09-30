using UnityEngine;

using System.Collections;



public class Udp : MonoBehaviour
{

    string buffer_ = "0_0_0";

    GameObject master_system;

    // Use this for initialization

    void Start()
    {

        master_system = GameObject.FindWithTag("Master_System");

    }



    // Update is called once per frame

    void Update()
    {



        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))

        {

            if (hit.collider.CompareTag("under") && Input.GetKeyDown(KeyCode.Mouse0))

            {

                int list_num = master_system.GetComponent<MasterSystem>().Server_Connent_num;

                buffer_ = "1_" + list_num.ToString() + "_" + hit.point.x.ToString() + "_0.5_" + hit.point.z.ToString();

                master_system.GetComponent<MasterSystem>().Server_Send(buffer_);

            }

        }

    }

}