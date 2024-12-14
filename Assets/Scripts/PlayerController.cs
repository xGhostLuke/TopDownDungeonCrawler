using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float xPos, yPos;
    private bool findStartPos;
    private MapGen MapGen;
    void Start()
    {
        MapGen = GameObject.FindGameObjectWithTag("Map").GetComponent<MapGen>();
        findStartPos = false;
    }

    void Update()
    {
        if(MapGen.getStatus()){
            initiatePlayer();
            movement();
            updateTransform();
        }
    }

    private void initiatePlayer(){
        if(!findStartPos){
            xPos = MapGen.getRoom("start").GetX();
            yPos = MapGen.getRoom("start").GetY();
            findStartPos = true;
        }
    }

    private void updateTransform(){
        transform.position = new Vector3(xPos, yPos, 0);
    }

    private void movement(){
        if(Input.GetKeyDown(KeyCode.UpArrow)){
            if(MapGen.getRoom((int)xPos, (int)yPos+1) != null){
                yPos++;
                Debug.Log("new Pos: " + xPos + yPos);
            }
        }
        if(Input.GetKeyDown(KeyCode.DownArrow)){
            if(MapGen.getRoom((int)xPos, (int)yPos-1) != null){
                yPos--;
                Debug.Log("new Pos: " + xPos + yPos);
            }
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            if(MapGen.getRoom((int)xPos+1, (int)yPos) != null){
                xPos++;
                Debug.Log("new Pos: " + xPos + yPos);
            }
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            if(MapGen.getRoom((int)xPos-1, (int)yPos) != null){
                xPos--;
                Debug.Log("new Pos: " + xPos + yPos);
            }
        }
    }
}
