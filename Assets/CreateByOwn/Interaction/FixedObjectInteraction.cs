using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public enum TransmitType
{
    None,
    Up,
    Down,
}
public class FixedObjectInteraction : MonoBehaviour
{
    private doorController dc; // 门的控制器
    private DrawerController drawerController; // 抽屉的控制器
    public Button openDoorBtn;
    public Sprite OpenDoorSprite;
    public Sprite LadderSprite;
    public Image SwitchPositionPanel;
    private TransmitType tranmitType;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            dc = other.GetComponent<doorController>();
            openDoorBtn.GetComponent<Image>().sprite = OpenDoorSprite;
            if (dc.isOpen)
            {
                openDoorBtn.GetComponentInChildren<Text>().text = "Press Q";
            }
            else
            {
                openDoorBtn.GetComponentInChildren<Text>().text = "Press Q";
            }
            openDoorBtn.gameObject.SetActive(true);
        }
        else if (other.CompareTag("Drawer"))
        {
            drawerController = other.GetComponent<DrawerController>();
        }
        if (other.gameObject.name.Contains("Ladder"))
        {
            openDoorBtn.gameObject.SetActive(true);
            openDoorBtn.GetComponent<Image>().sprite = LadderSprite;
            ladderTransmit = other.transform.parent.GetComponent<LadderTransmit>();
            if (ladderTransmit != null)
            {
                Debug.Log("可以传送");
            }
            if (other.gameObject.name.Contains("bottomCollider"))
            {
                openDoorBtn.GetComponentInChildren<Text>().text = "Press Q";
                tranmitType = TransmitType.Up;
            }
            else if(other.gameObject.name.Contains("upCollider"))
            {
                openDoorBtn.GetComponentInChildren<Text>().text = "Press Q";
                tranmitType = TransmitType.Down;
            }
        }
    }
    private LadderTransmit ladderTransmit;
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            dc = null;
            openDoorBtn.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Drawer"))
        {
            drawerController = null;
        }else if (other.gameObject.name.Contains("Ladder"))
        {
            ladderTransmit = null;
            openDoorBtn.gameObject.SetActive(false);
            tranmitType = TransmitType.None;
        }
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 50), "ladderTransmit:"+(ladderTransmit==null));
        GUI.Label(new Rect(10, 60, 200, 50), "tranmitType:"+tranmitType);
    }
    public void ToggleDoor()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (dc != null)
            {
                dc.ToggleDoor();
            }
        }
    }
    public void ToggleLadder(Rigidbody rb)
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (ladderTransmit != null)
            {
                Transmit(rb);
            }
        }
    }
    void Transmit(Rigidbody rb)
    {
        Vector3 otherPos=Vector3.zero;
        if (tranmitType==TransmitType.Up)
        {
            //otherPos = other.gameObject.transform.parent.Find("Ladder_upCollider").position;
            //transform.position=ladderTransmit.upPos.position;
            rb.MovePosition(ladderTransmit.upPos.position);
        }
        else if(tranmitType==TransmitType.Down)
        {
            //otherPos = other.gameObject.transform.parent.Find("Ladder_bottomCollider").position;
            //transform.position=ladderTransmit.bottomPos.position;
            rb.MovePosition(ladderTransmit.bottomPos.position);
        }
        /*Debug.Log("otherPos:"+otherPos);
        float offsetSize = 0.75f;
        Vector3 transmitPos=Vector3.zero;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <=1; j++)
            {
                if(i==0&&j==0)continue;
                Vector3 offset=new Vector3(offsetSize*i, 0, offsetSize*j);
                Vector3 offset_up = new Vector3(0, 0, 0.3f);
                Vector3 rayPos= otherPos +offset_up+ offset;
                Debug.DrawRay(rayPos, Vector3.down, Color.red, 1f);
                RaycastHit hit;
                if (Physics.Raycast(rayPos, Vector3.down, out hit, 5f))
                {
                    if (hit.collider.gameObject.name.Contains("Floor"))
                    {
                        Debug.Log("射线碰撞到了Floor");
                        transmitPos = hit.point + Vector3.up * 0.01f;
                        Debug.Log("碰撞点位置：" + transmitPos);
                        break;
                    }
                }
            }
            if(transmitPos!=Vector3.zero) break;
        }
       transform.position = transmitPos;*/
        SwitchPosition();

    }
    void SwitchPosition()
    {
        SwitchPositionPanel.GameObject().SetActive(true);
        Invoke("SetActiveSwitchPositionPanel",1f);
    }
    public void SetActiveSwitchPositionPanel()
    {
        SwitchPositionPanel.gameObject.SetActive(false);
    }
    public void ToggleDrawer()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (drawerController != null)
            {
                drawerController.ToggleDrawer();
            }
        }
    }
}