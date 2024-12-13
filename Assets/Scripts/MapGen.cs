using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab, floorPrefab, map;
    [SerializeField] private int[,] roomArray = new int[5, 5];
    [SerializeField] private List<GameObject> rooms = new List<GameObject>();
    [SerializeField] private HashSet<Vector3> instantiatedFloors = new HashSet<Vector3>(); 
    private int rows, columns;

    private bool hasEnd, hastStart;

    void Start()
    {
        rows = roomArray.GetLength(0);
        columns = roomArray.GetLength(1);
        map = GameObject.FindGameObjectWithTag("Map");
        hasEnd = false;
        hastStart = false;

        GenerateMap();
    }

    public void GenerateMap(){
        ClearMap();
        GenerateRooms();
        GenerateFloors();
        correctFloors();
        checkForDistantRooms();
        findStartEnd();
    }

    private void GenerateRooms()
    {
        GameObject clone;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int x = UnityEngine.Random.Range(0, rows);
                int y = UnityEngine.Random.Range(0, columns);

                if (roomArray[x, y] == 0)
                {
                    roomArray[x, y] = 1;

                    clone = Instantiate(roomPrefab, new Vector3(x, y, 0), Quaternion.identity, map.transform);
                    clone.name = $"Room({x}|{y})";
                    clone.GetComponent<Room>().SetX(x);
                    clone.GetComponent<Room>().SetY(y);
                    clone.GetComponent<Room>().SetType("Default");
                    rooms.Add(clone);
                }
            }
        }
    }

    private void GenerateFloors()
    {
        foreach (GameObject _room in rooms)
        {
            Room room = _room.GetComponent<Room>();
            if (CheckNeighbor(room, "Right")){
                Vector3 floorPosition = new Vector3(room.GetX() + 0.5f, room.GetY(), 0);
                if (!instantiatedFloors.Contains(floorPosition)) 
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 90f), map.transform).name = "Floor";
                    instantiatedFloors.Add(floorPosition); 
                    room.SetFloors(room.GetFloors() + "Right");
                }
            }
            if (CheckNeighbor(room, "Left"))
            {
                Vector3 floorPosition = new Vector3(room.GetX() - 0.5f, room.GetY(), 0);
                if (!instantiatedFloors.Contains(floorPosition))
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 90f), map.transform).name = "Floor";
                    instantiatedFloors.Add(floorPosition);
                    room.SetFloors(room.GetFloors() + "Left");
                }
            }
            if (CheckNeighbor(room, "Top"))
            {
                Vector3 floorPosition = new Vector3(room.GetX(), room.GetY() + 0.5f, 0);
                if (!instantiatedFloors.Contains(floorPosition))
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 0f), map.transform).name = "Floor";
                    instantiatedFloors.Add(floorPosition);
                    room.SetFloors(room.GetFloors() + "Top");
                }
            }
            if (CheckNeighbor(room, "Down"))
            {
                Vector3 floorPosition = new Vector3(room.GetX(), room.GetY() - 0.5f, 0);
                if (!instantiatedFloors.Contains(floorPosition))
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 0f), map.transform).name = "Floor";
                    instantiatedFloors.Add(floorPosition);
                    room.SetFloors(room.GetFloors() + "Down");
                }
            }
        }
    }

    private bool CheckNeighbor(Room _room, string direction)
    {
        int x = (int)_room.GetX();
        int y = (int)_room.GetY();

        switch (direction)
        {
            case "Right":
                if (x + 1 < columns && roomArray[x + 1, y] == 1)
                {
                    return true;
                }
                break;

            case "Left":
                if (x - 1 >= 0 && roomArray[x - 1, y] == 1)
                {
                    return true;
                }
                break;

            case "Top":
                if (y + 1 < rows && roomArray[x, y + 1] == 1)
                {
                    return true;
                }
                break;

            case "Down":
                if (y - 1 >= 0 && roomArray[x, y - 1] == 1)
                {
                    return true;
                }
                break;
        }

        return false;
    }

    private void correctFloors(){
        foreach (GameObject _room in rooms){
            Room room = _room.GetComponent<Room>();
            if (CheckNeighbor(room, "Top") && !room.GetFloors().Contains("Top")){
                room.SetFloors(room.GetFloors() + "Top");
            }
            if (CheckNeighbor(room, "Down") && !room.GetFloors().Contains("Down")){
                room.SetFloors(room.GetFloors() + "Down");
            }
            if (CheckNeighbor(room, "Left") && !room.GetFloors().Contains("Left")){
                room.SetFloors(room.GetFloors() + "Left");
            }
            if (CheckNeighbor(room, "Right") && !room.GetFloors().Contains("Right")){
                room.SetFloors(room.GetFloors() + "Right");
            }

        }
    }

    private void findStartEnd(){
        int startX = UnityEngine.Random.Range(0, rows);
        int startY = UnityEngine.Random.Range(0, columns);
        int endX = UnityEngine.Random.Range(0, rows);
        int endY = UnityEngine.Random.Range(0, columns);
        foreach (GameObject _room in rooms){
            Room room = _room.GetComponent<Room>();
            if(startX == room.GetX() && startY == room.GetY() && !hastStart){
                room.SetType("start");
                _room.GetComponent<SpriteRenderer>().color = Color.green;
                hastStart = true;

                Debug.Log("start at: " + startX + "" + startY + "");
            }
            if(endX == room.GetX() && endY == room.GetY() && !hasEnd){
                room.SetType("end");
                _room.GetComponent<SpriteRenderer>().color = Color.red;
                hasEnd = true;

                Debug.Log("end at: " + endX + " " + endY + "");
            }
        }
        if(!hastStart || !hasEnd){
            findStartEnd();
        }
    }

    private void checkForDistantRooms(){
        bool needToGenerateNewMap = false;
        foreach (GameObject _room in rooms){
            Room room = _room.GetComponent<Room>(); 
            if (room.GetFloors().Equals("")){
                needToGenerateNewMap = true;
                Debug.Log("Found distant Room");
                
            }
        }
        if (needToGenerateNewMap){
            GenerateMap();
        }
        
    }

    private void ClearMap()
    {   
        hastStart = false;
        hasEnd = false;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                roomArray[i, j] = 0;
            }
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Floor")){
            Destroy(obj);
        }
        foreach (GameObject _room in rooms){
            Destroy(_room);   
        }
        instantiatedFloors.Clear();
        rooms.Clear();  
        
    }
}
