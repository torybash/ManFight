using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LevelControl : MonoBehaviour {





	//Variables
	List<Color> teamColors = null;
	MapTile[][] currLvl;
	public int width, height;


	//References
	LevelCreatorControl lvlCrtrCtrl;
	NetworkManagerControl netManCtrl;
	DarknessFogControl fogCtrl;
	MapMaster mapMstr;
	public Dictionary<int, Transform> startingPositions = new Dictionary<int, Transform>();



	void Awake(){
		fogCtrl = GetComponent<DarknessFogControl>();
		netManCtrl = GetComponent<NetworkManagerControl>();
		lvlCrtrCtrl = GetComponent<LevelCreatorControl>();
		mapMstr = GameObject.Find("LevelLoadingControl").GetComponent<MapMaster>();
	}



	public void SpawnLevel(){
		MapTile[][] lvl = mapMstr.GetLevel(0);

		currLvl = lvl;
		
		width = lvl.Length;
		height = lvl[0].Length;
		
		fogCtrl.InitFog(lvl);


		lvlCrtrCtrl.SpawnLevel(lvl);




	}

//	private SpriteType[][] GetLevel(int idx){
//		SpriteType[][] lvl = mapMstr.GetLevel(idx);
//
//
//
//		DEBUG LEVEL
//		int width = 14,  height = 14;
//		if (idx == -1) {
//			lvl = new SpriteType[width][];
//			for (int x = 0; x < width; x++) {
//				lvl[x] = new SpriteType[height];
//				for (int y = 0; y < height; y++) {
//
//					if (x == 0 || x == width-1 || y == 0 || y == height-1){
//						lvl[x][y] = SpriteType.ROCK;
//					}else{
//						lvl[x][y] = SpriteType.EMPTY;
//					}
//
//					if (x == 3 && y > 5) lvl[x][y] = SpriteType.ROCK;
//					if (x == 6 && y < 8) lvl[x][y] = SpriteType.ROCK;
//					if (x == 10 && y > 7) lvl[x][y] = SpriteType.ROCK;
//				}
//			}
//
//			lvl[1][1] = SpriteType.STARTING_POS;
//			lvl[width-2][1] = SpriteType.STARTING_POS;
//			lvl[1][height-2] = SpriteType.STARTING_POS;
//			lvl[width-2][height-2] = SpriteType.STARTING_POS;
//
//			
//		}
//		return lvl;
//	}


//	public StartPosition PollRandomStartingPos(){
//		int idx = Tools.RndRange(0, startingPositions.Count);
//		StartPosition result = startingPositions[idx].GetComponent<StartPosition>();
//		startingPositions.Remove(startingPositions[idx]);
//		return result;
//	}

	public Transform GetStartingPosition(int playerID){
		return startingPositions[playerID];
	}


	public static Vector2 LevelToWorldPos(int x, int y){
				
		Vector2 result = new Vector2(x + 0.5f, y + 0.5f);
		return result;
	}


	private Color GetNextColor(){
		if (teamColors == null){
			teamColors = new List<Color>();
			teamColors.Add(Color.red);
			teamColors.Add(Color.blue);
			teamColors.Add(Color.green);
			teamColors.Add(Color.yellow);
		}

		Color returnColor = teamColors[teamColors.Count-1];
		teamColors.Remove(returnColor);

		return returnColor;
	}



	public List<Vector2> AStarPath(Vector2 startPos, Vector2 goalPos){

		int goalPosX = (int) goalPos.x;
		int goalPosY = (int) goalPos.y;
		
		Node startNode = new Node((int)startPos.x, (int)startPos.y, 0, null);


		HashSet<Node> closedNodes = new HashSet<Node>();
		HeapPriorityQueue<Node> q = new HeapPriorityQueue<Node>(100000);
        List<Vector2> path = null;                            

		q.Enqueue(startNode, AStarHeuristic(startNode.x, startNode.y, goalPosX, goalPosY));

		while (q.Count > 0){

			Node currNode = q.Dequeue();
//			print ("currNode: " + currNode.x + ", " + currNode.y + " - moves: " + currNode.g + ", h: " + AStarHeuristic(currNode.x, currNode.y, goalPosX, goalPosY) + ", prio: " + currNode.Priority); 
			if (currNode.x == goalPosX && currNode.y == goalPosY){
				return GetPathThroughNodes(currNode);
			}

			closedNodes.Add(currNode);

			foreach (Node neighbor in NeighborNodes(currNode)) {
				if (closedNodes.Contains(neighbor)){
//					print ("node was in closedNodes!");
					continue;
				}

				if (q.Contains(neighbor)){
//					print ("node was in closedNodes!");
					continue;
				}
//				float tentativeG = 


				q.Enqueue(neighbor, neighbor.g + AStarHeuristic(currNode.x, currNode.y, goalPosX, goalPosY));

//				print ("neighbor added: " + neighbor.x + ", " + neighbor.y + " - moves: " + neighbor.g + ", h: " + AStarHeuristic(neighbor.x, neighbor.y, goalPosX, goalPosY) + ", prio: " + neighbor.Priority); 

			}

//		add current to closedset
//		for each neighbor in neighbor_nodes(current)
//			if neighbor in closedset
//				continue
//					tentative_g_score := g_score[current] + dist_between(current,neighbor)
//					
//					if neighbor not in openset or tentative_g_score < g_score[neighbor] 
//					came_from[neighbor] := current
//					g_score[neighbor] := tentative_g_score
//					f_score[neighbor] := g_score[neighbor] + heuristic_cost_estimate(neighbor, goal)
//					if neighbor not in openset
//						add neighbor to openset

										
										//			if (closedNodes.Contains(currNode)){
										//				continue;
										//			}
										
										//			Node


//			q.Enqueue(startNode, AStarHeuristic(startNode.x, startNode.y, goalPosX, goalPosY));

//			closedNodes.Add(currNode);
		}



		return path;

	}


	List<Vector2> GetPathThroughNodes(Node endNode){
		List<Vector2> path = new List<Vector2>();

		Node currNode = endNode;
		while (true){
			if (currNode.cameFromNode == null){
				path.Reverse();
				return path; //reached start 
			}
		
			int thisX = currNode.x; int thisY = currNode.y;
			int prevX = currNode.cameFromNode.x; int prevY = currNode.cameFromNode.y;
			path.Add(new Vector2(thisX - prevX, thisY - prevY));

		    currNode = currNode.cameFromNode;
		}
	}


	float AStarHeuristic(int startX, int startY, int endX, int endY){
//		Debug.Log("AStarHeuristic - start: " + startX + ", " + startY + " -- end: " + endX + ", " + endY + " = " + Vector2.Distance(new Vector2(startX, startY), new Vector2(endX, endY)) );
		return Vector2.Distance(new Vector2(startX, startY), new Vector2(endX, endY));
	}

	List<Node> NeighborNodes(Node node){
		List<Node> nodes = new List<Node>();
		for (int x = -1; x < 2; x++) {
			for (int y = -1; y < 2; y++) {
				if ((x == 0 && y == 0) || (x != 0 && y != 0)) continue;

				int tileX = node.x + x; int tileY = node.y + y;

				if (tileX < 0 || tileY < 0 || tileX >= currLvl.Length || tileY >= currLvl[0].Length) continue;
				//CHECK FOR UNWALKABLE/UNPASSEABLE/WALL/ROCK
				if (currLvl[tileX][tileY].sprites.Contains(SpriteType.ROCK)) continue;
				Node newNode = new Node(tileX, tileY, node.g + 1f - 0.0001f ,node);
				nodes.Add(newNode);
			}
		}
		return nodes;
	}



	public bool IsValidRobotPlacement(int x, int y){
		int playerID = netManCtrl.localPlayer.playerID;

		MapTile tile = currLvl[x][y];
		foreach (SpriteType sp in tile.sprites) if (sp == SpriteType.ROCK) return false;
		if (tile.playerID != playerID) return false;
		return true;
	}
}


public class Node : PriorityQueueNode{
	public int x, y;
	public float g; //amount of moves, basically
	public Node cameFromNode;

	public Node(int x, int y, float g, Node cameFromNode){
		this.x = x;
		this.y = y;
		this.g = g;
		this.cameFromNode = cameFromNode;
	}

	public override bool Equals(object obj){
		Node other = (Node)obj;
		if (x == other.x && y == other.y) return true;
		return false;
	}
	
	public override int GetHashCode(){
		int prime = 31;
		int result = 1;
		result = prime * result + x;
		result = prime * result + y;
		return result;
		
	}
}

public enum SpriteType{
	ROCK,
	STARTING_POS,
	STARTING_FIELD,
	EMPTY
}
