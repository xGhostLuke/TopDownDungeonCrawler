using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private int[,] roomArray = new int[5, 5];
    private List<Room> rooms = new List<Room>();
    private HashSet<Vector3> instantiatedFloors = new HashSet<Vector3>();  // Track instantiated floors
    private int rows, columns;

    void Start()
    {
        rows = roomArray.GetLength(0);
        columns = roomArray.GetLength(1);

        ClearMap();
        GenerateRooms();
        GenerateFloors();
    }

    private void GenerateRooms()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int x = Random.Range(0, rows);
                int y = Random.Range(0, columns);

                if (roomArray[x, y] == 0)
                {
                    roomArray[x, y] = 1;

                    Instantiate(roomPrefab, new Vector3(x, y, 0), Quaternion.identity).name = $"Room({x}|{y})";

                    rooms.Add(new Room(x, y, "Default"));
                }
            }
        }
    }

    private void GenerateFloors()
    {
        foreach (Room room in rooms)
        {
            if (CheckNeighbor(room, "Right"))
            {
                Vector3 floorPosition = new Vector3(room.GetX() + 0.5f, room.GetY(), 0);
                if (!instantiatedFloors.Contains(floorPosition))  // Check if the floor is already instantiated
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 90f));
                    instantiatedFloors.Add(floorPosition);  // Mark the floor as instantiated
                }
            }
            if (CheckNeighbor(room, "Left"))
            {
                Vector3 floorPosition = new Vector3(room.GetX() - 0.5f, room.GetY(), 0);
                if (!instantiatedFloors.Contains(floorPosition))
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 90f));
                    instantiatedFloors.Add(floorPosition);
                }
            }
            if (CheckNeighbor(room, "Top"))
            {
                Vector3 floorPosition = new Vector3(room.GetX(), room.GetY() + 0.5f, 0);
                if (!instantiatedFloors.Contains(floorPosition))
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 0f));
                    instantiatedFloors.Add(floorPosition);
                }
            }
            if (CheckNeighbor(room, "Down"))
            {
                Vector3 floorPosition = new Vector3(room.GetX(), room.GetY() - 0.5f, 0);
                if (!instantiatedFloors.Contains(floorPosition))
                {
                    Instantiate(floorPrefab, floorPosition, transform.rotation * Quaternion.Euler(0f, 0f, 0f));
                    instantiatedFloors.Add(floorPosition);
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

    private void ClearMap()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                roomArray[i, j] = 0;
            }
        }
    }
}
