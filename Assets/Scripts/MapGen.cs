using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab, floorPrefab, map;
    [SerializeField] private int[,] roomArray;
    [SerializeField] private List<GameObject> rooms = new List<GameObject>();
    [SerializeField] private HashSet<Vector3> instantiatedFloors = new HashSet<Vector3>(); 
    [SerializeField] private int rows, columns, startX, startY, endX, endY, offset;
    private bool hasEnd, hastStart, needToGenerateNewMap, finishedGenerating;

    void Start()
    {
        roomArray = new int[rows, columns];
        map = GameObject.FindGameObjectWithTag("Map");
        hasEnd = false;
        hastStart = false;
        finishedGenerating = false;
        GenerateMap();
    }

    public void GenerateMap(){
        needToGenerateNewMap = false;
        ClearMap();
        GenerateRooms();
        GenerateFloors();
        correctFloors();
        findStartEnd();
        checkMap();
        finishedGenerating = true;
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

                    clone = Instantiate(roomPrefab, new Vector3(x-offset, y-offset, 0), Quaternion.identity, map.transform);
                    clone.name = $"Room({x}|{y})";
                    clone.GetComponent<Room>().SetX(x-offset);
                    clone.GetComponent<Room>().SetY(y-offset);
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
            if (CheckNeighbor(room, "R")){
                Vector3 floorPosition = new Vector3(room.GetX() + 0.5f, room.GetY(), 0);
                if (!instantiatedFloors.Contains(floorPosition)) 
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 90f), map.transform).name = "Floor";
                    instantiatedFloors.Add(floorPosition); 
                    room.SetFloors(room.GetFloors() + "R");
                }
            }
            if (CheckNeighbor(room, "L"))
            {
                Vector3 floorPosition = new Vector3(room.GetX() - 0.5f, room.GetY(), 0);
                if (!instantiatedFloors.Contains(floorPosition))
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 90f), map.transform).name = "Floor";
                    instantiatedFloors.Add(floorPosition);
                    room.SetFloors(room.GetFloors() + "L");
                }
            }
            if (CheckNeighbor(room, "T"))
            {
                Vector3 floorPosition = new Vector3(room.GetX(), room.GetY() + 0.5f, 0);
                if (!instantiatedFloors.Contains(floorPosition))
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 0f), map.transform).name = "Floor";
                    instantiatedFloors.Add(floorPosition);
                    room.SetFloors(room.GetFloors() + "T");
                }
            }
            if (CheckNeighbor(room, "D"))
            {
                Vector3 floorPosition = new Vector3(room.GetX(), room.GetY() - 0.5f, 0);
                if (!instantiatedFloors.Contains(floorPosition))
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 0f), map.transform).name = "Floor";
                    instantiatedFloors.Add(floorPosition);
                    room.SetFloors(room.GetFloors() + "D");
                }
            }
        }
    }

    private bool CheckNeighbor(Room _room, string direction)
    {
        int x = (int)_room.GetX() + offset;
        int y = (int)_room.GetY() + offset;

        switch (direction)
        {
            case "R":
                if (x + 1 < columns && roomArray[x + 1, y] == 1)
                {
                    return true;
                }
                break;

            case "L":
                if (x - 1 >= 0 && roomArray[x - 1, y] == 1)
                {
                    return true;
                }
                break;

            case "T":
                if (y + 1 < rows && roomArray[x, y + 1] == 1)
                {
                    return true;
                }
                break;

            case "D":
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
            if (CheckNeighbor(room, "T") && !room.GetFloors().Contains("T")){
                room.SetFloors(room.GetFloors() + "T");
            }
            if (CheckNeighbor(room, "D") && !room.GetFloors().Contains("D")){
                room.SetFloors(room.GetFloors() + "D");
            }
            if (CheckNeighbor(room, "L") && !room.GetFloors().Contains("L")){
                room.SetFloors(room.GetFloors() + "L");
            }
            if (CheckNeighbor(room, "R") && !room.GetFloors().Contains("R")){
                room.SetFloors(room.GetFloors() + "R");
            }

        }
    }

    private void findStartEnd(){
        startX = UnityEngine.Random.Range(0-offset, rows);
        startY = UnityEngine.Random.Range(0-offset, columns);
        endX = UnityEngine.Random.Range(0-offset, rows);
        endY = UnityEngine.Random.Range(0-offset, columns);
        while(startX == endX && startY == endY){
            endX = UnityEngine.Random.Range(0-offset, rows);
            endY = UnityEngine.Random.Range(0-offset, columns);
        }  
        foreach (GameObject _room in rooms){
            Room room = _room.GetComponent<Room>();
            if(startX == room.GetX() && startY == room.GetY() && !hastStart){
                room.SetType("start");
                _room.GetComponent<SpriteRenderer>().color = Color.green;
                hastStart = true;

                Debug.Log("start at: " + startX + " " + startY + "");
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

    private void checkMap(){
        int oneWayRooms = 0;
        int startEndRooms = 0;
        foreach (GameObject _room in rooms){
            Room room = _room.GetComponent<Room>();
            if(room.GetFloors().Length <= 1){
                oneWayRooms++;
            }
            if (room.GetFloors().Equals("")){
                needToGenerateNewMap = true;
            }
            if (room.GetType().Equals("start") || room.GetType().Equals("end")){
                startEndRooms++;
            }
        }
        if (oneWayRooms > 2 || needToGenerateNewMap || startEndRooms != 2){
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

    public Room getRoom(int x, int y){
        foreach (GameObject _room in rooms){
            Room room = _room.GetComponent<Room>();
            if (x == room.GetX() && y == room.GetY()){
                return room;
            }
        }
        return null;
    }

    public Room getRoom(string str){
        foreach (GameObject _room in rooms){
            Room room = _room.GetComponent<Room>();
            if (room.GetType().Equals(str)){
                return room;
            }
        }
        Debug.Log("No Room found of type: " + str);
        return null;
    }

    public bool getStatus(){
        return finishedGenerating;
    }
}
