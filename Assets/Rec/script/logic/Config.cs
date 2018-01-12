using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour {
	public const int width = 18;
	public const int height = 32;
	public const int halfWidth = 9;
	public const int halfHeight = 16;
	public const int unit = 60;
	public const int FLAG_EMPTY = 0;
	public const int FLAG_TILE = -1;
	public const int FLAG_TAIL = 99;
	public const int FLAG_WALL = -2;
	public static Coord[] directionArray = {new Coord(0,1), new Coord(1,0), new Coord(0, -1), new Coord(-1,0)};
	public static Quaternion[] RotationArray = {Quaternion.Euler(new Vector3(0, 0, 0)),
		Quaternion.Euler(new Vector3(0, 0, -90)),
		Quaternion.Euler(new Vector3(0, 0, -180)),
		Quaternion.Euler(new Vector3(0, 0, -270))};
	public const string TAG_WALL = "Wall";
	public const string TAG_TILE = "Tile";
	public const string TAG_HEAD = "Head";
	public const string TAG_TAIL = "Tail";
	public const string TAG_ENEMY = "Enemy";

}

public class Coord
{
	public int x;
	public int y;
	public Coord()
	{
		x = 0;
		y = 0;
	}
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

public enum MstType{randomWalk=1, darkEdge=2, darkRandom, breaker};
public enum BonusType{speedUp=1, speedDown=2, freeze, eat};