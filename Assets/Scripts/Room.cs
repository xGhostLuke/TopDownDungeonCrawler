using System;
using UnityEngine;

[System.Serializable]
public class Room : MonoBehaviour
{
    private float x, y;
    [SerializeField] private string type, floors;

    public Room(float x, float y, string type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.floors = "";
    }

    public float GetX() => x;
    public void SetX(float value) => x = value;

    public float GetY() => y;
    public void SetY(float value) => y = value;

    public new string GetType() => type;
    public void SetType(string value) => type = value;

    public void SetFloors(string value) => floors = value;
    public String GetFloors(){
        return floors;  
    } 
}
