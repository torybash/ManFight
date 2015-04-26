using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class TilePos : IEquatable<TilePos>{

	public int x, y;

	public TilePos(int x, int y){
		this.x = x;
		this.y = y;
	}

	public bool Equals(TilePos tilePos){
		return true;
	}

	public override bool Equals(System.Object obj){

		if (obj == null) return false;

		
		TilePos tilePos = (TilePos) obj;
		if (tilePos == null){
			return false;
		}else{
			if (tilePos.x == x && tilePos.y == y){
				return true;
			}
			return false;
		}
	}   


	public override int GetHashCode(){
		return (x * 2) + (y*3);

	}
}
