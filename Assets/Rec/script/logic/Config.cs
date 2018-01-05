using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour {
	public const int width = 18;
	public const int height = 32;
	public const int unit = 60;
	public const int FLAG_EMPTY = 0;
	public const int FLAG_TILE = -1;
	public const int FLAG_TAIL = 99;
	public const int FLAG_WALL = -2;
	public static Coord[] directionArray = {new Coord(0,1), new Coord(1,0), new Coord(0, -1), new Coord(-1,0)};
	public static Quaternion[] RotationArray = {Quaternion.Euler(new Vector3(0, 0, 180)), 
		Quaternion.Euler(new Vector3(0, 0, 90)), 
		Quaternion.Euler(Vector3.zero),
				Quaternion.Euler(new Vector3(0, 0, 270))};
	public const string TAG_WALL = "Wall";
	public const string TAG_TILE = "Tile";


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class Coord
{
	public int x;
	public int y;
	public Coord(int ix, int iy)
	{
		x = ix;
		y = iy;
	}

	public static Coord operator +(Coord lhs, Coord rhs)
	       {
		Coord result = new Coord(lhs.x, lhs.y);
		            result.x += rhs.x;
		            result.y += rhs.y;
		            return result;
		        }
}

public enum MstType{randomWalk, darkEdge, darkRandom, breaker};
