using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�� �̵� �Ѱ� �Ŵ��� #������
public class Map_Manager : MonoBehaviour
{
    [SerializeField]
    private GameObject Obj_Player;
    [SerializeField]
    private GameObject[] Obj_Portals;

    [HideInInspector]
    public bool IsOnPortal = false;
    [HideInInspector]
    public GameObject Which_Portal;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Use_Portal();
    }

    private void Testing()
    {
        //Fucking Gitgub
    }
    void Use_Portal() //��Ż ��� (W Ű) (IsOnPortal = true �� ��) #������ �ӽ� �ּ� �߰�#
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (IsOnPortal)
            {
                if (Which_Portal == Obj_Portals[0])
                {
                    Obj_Player.transform.position = Obj_Portals[1].transform.position;
                }
                else if (Which_Portal == Obj_Portals[1])
                {
                    Obj_Player.transform.position = Obj_Portals[0].transform.position;
                }
            }
        }
    }
}
