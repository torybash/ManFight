using UnityEngine;
using System.Collections;

public class DarknessFog : MonoBehaviour {

	int fogSizeX = 32; 
	int fogSizeY = 32; 

	byte[,] fogMap = new byte[32,32];

	Vector2 fogDisplacement = new Vector2(-16, -16);

	// Use this for initialization
	void Start () {
		transform.Translate(0,0,-0.05f,Space.World);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void UpdateDarknessFog(int x, int y, byte[,] exploredMap, int mapWidth, int mapHeight){


//		Debug.Log("UpdateDarknessFog - x: "+ x + ", y: " + y + " exploredMap: "+ exploredMap + ", mapWidth: " + mapWidth + ", mapHeight: " + mapHeight);

		int startFogX = x - fogSizeX/2;
		int startFogY = y - fogSizeY/2;


		for (int i = 0; i < fogSizeX; i++) {
			for (int j = 0; j < fogSizeY; j++) {
				int realX = startFogX + i;
				int realY = startFogY + j;

//				Debug.Log("realX: " + realX + ", realY" + realY);

//				if (realX < 0 || realX > mapWidth - fogSizeX) continue;
//				if (realY < 0 || realY > mapHeight - fogSizeY) continue;
				if (realX < 0 || realX >= mapWidth) continue;
				if (realY < 0 || realY >= mapHeight) continue; 


//				Debug.Log("realX: " + realX + ", realY" + realY + " i, j: " + i + ", " + j);
				fogMap[i,j] = exploredMap[realX, realY];

//				Debug.Log("fogMap[i,j]: " + i + ", " + j + ", = " + fogMap[i,j]);
			}
		}


		UpdateMesh();

		transform.position = new Vector2(startFogX + 0.5f, startFogY + 0.5f);
	}


	void UpdateMesh(){
		
		Mesh mesh = new Mesh();
		
		Vector3[] vertices = new Vector3[fogSizeX*fogSizeY*5];
		for (int i = 0; i < fogSizeX; i++){
			for (int j = 0; j < fogSizeY; j++){
				vertices[i*5+j*5*fogSizeX]   =new Vector3(1+i,j+1, 0);
				vertices[i*5+1+j*5*fogSizeX] =new Vector3(1+i,j,0);
				vertices[i*5+2+j*5*fogSizeX] =new Vector3(i,j+1, 0);
				vertices[i*5+3+j*5*fogSizeX] =new Vector3(i,j,0);
				vertices[i*5+4+j*5*fogSizeX] =new Vector3(i+0.5f,j+0.5f,0);
			}
		}

		Vector2[] uv = new Vector2[fogSizeX*fogSizeY*5];
		for (int i = 0; i < fogSizeX; i++){
			for (int j = 0; j < fogSizeY; j++){
				
				int xInt = 0;
				int yInt = 0;

				float startX = xInt * 0.25f;
				float endX = xInt * 0.25f + 0.25f;
				float startY = yInt * 0.25f;
				float endY = yInt * 0.25f + 0.25f;

				
				uv[i*4+j*4*fogSizeX]     =new Vector2(endX-0.01f,endY-0.01f);
				uv[i*4+1+j*4*fogSizeX]   =new Vector2(endX-0.01f,startY);
				uv[i*4+2+j*4*fogSizeX]   =new Vector2(startX,endY-0.01f);
				uv[i*4+3+j*4*fogSizeX]   =new Vector2(startX,startY);
				uv[i*4+4+j*4*fogSizeX]   =new Vector2(1,1);
				
				
				//				uv[i*4+j*4*size]     =new Vector2(0.249f,0.249f);
				//				uv[i*4+1+j*4*size]   =new Vector2(0.249f,0);
				//				uv[i*4+2+j*4*size]   =new Vector2(0,0.249f);
				//				uv[i*4+3+j*4*size]   =new Vector2(0,0);
			}
		}
		
		int[] triangles= new int[fogSizeX*fogSizeY*12];
		for (int i = 0; i < fogSizeX; i++){
			for (int j = 0; j < fogSizeY; j++){
				triangles[i*12+j*12*fogSizeX]      =0+i*5+j*5*fogSizeX;
				triangles[i*12+1+j*12*fogSizeX]    =1+i*5+j*5*fogSizeX;
				triangles[i*12+2+j*12*fogSizeX]    =4+i*5+j*5*fogSizeX;
				triangles[i*12+3+j*12*fogSizeX]    =1+i*5+j*5*fogSizeX;
				triangles[i*12+4+j*12*fogSizeX]    =3+i*5+j*5*fogSizeX;
				triangles[i*12+5+j*12*fogSizeX]    =4+i*5+j*5*fogSizeX;

				triangles[i*12+6+j*12*fogSizeX]    =3+i*5+j*5*fogSizeX;
				triangles[i*12+7+j*12*fogSizeX]    =2+i*5+j*5*fogSizeX;
				triangles[i*12+8+j*12*fogSizeX]    =4+i*5+j*5*fogSizeX;
				triangles[i*12+9+j*12*fogSizeX]    =2+i*5+j*5*fogSizeX;
				triangles[i*12+10+j*12*fogSizeX]   =0+i*5+j*5*fogSizeX;
				triangles[i*12+11+j*12*fogSizeX]   =4+i*5+j*5*fogSizeX;

			}
		}
		
		
		Color[] colors = new Color[fogSizeX*fogSizeY*5];
		
		for (int i = 0; i < fogSizeX-1; i++){
			for (int j = 0; j < fogSizeY-1; j++){
				
				float alpha1 = 1 - fogMap[i+1,j+1]/2f;
				float alpha2 = 1 - fogMap[i+1,j]/2f;
				float alpha3 = 1 - fogMap[i,j+1]/2f;
				float alpha4 = 1 - fogMap[i,j]/2f;
//				float alpha1 = 0.5f;
//				float alpha2 = 0.5f;
//				float alpha3 = 0.5f;
//				float alpha4 = 0.5f;
				
				
				colors[i*5+j*5*fogSizeX]     =new Color(1, 1, 1, alpha1);
				colors[i*5+1+j*5*fogSizeX]   =new Color(1, 1, 1, alpha2);
				colors[i*5+2+j*5*fogSizeX]   =new Color(1, 1, 1, alpha3);
				colors[i*5+3+j*5*fogSizeX]   =new Color(1, 1, 1, alpha4);
				colors[i*5+4+j*5*fogSizeX]   =new Color(1, 1, 1, (alpha1+alpha2+alpha3+alpha4)/4f);
				
				
				//				uv[i*4+j*4*size]     =new Vector2(0.249f,0.249f);
				//				uv[i*4+1+j*4*size]   =new Vector2(0.249f,0);
				//				uv[i*4+2+j*4*size]   =new Vector2(0,0.249f);
				//				uv[i*4+3+j*4*size]   =new Vector2(0,0);
			}
		}
		
		
		
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.colors = colors;
		mesh.RecalculateBounds();
		
		GetComponent<MeshFilter>().mesh = mesh;
	}


//	void UpdateMesh(){
//		
//		Mesh mesh = new Mesh();
//		
//		Vector3[] vertices = new Vector3[fogSizeX*fogSizeY*4];
//		for (int i = 0; i < fogSizeX; i++){
//			for (int j = 0; j < fogSizeY; j++){
//				vertices[i*4+j*4*fogSizeX]   =new Vector3(1+i,j+1, 0);
//				vertices[i*4+1+j*4*fogSizeX] =new Vector3(1+i,j,0);
//				vertices[i*4+2+j*4*fogSizeX] =new Vector3(i,j+1, 0);
//				vertices[i*4+3+j*4*fogSizeX] =new Vector3(i,j,0);
//			}
//		}
//		
//		Vector2[] uv = new Vector2[fogSizeX*fogSizeY*4];
//
//		for (int i = 0; i < fogSizeX; i++){
//			for (int j = 0; j < fogSizeY; j++){
//				
//				int xInt = 0;
//				int yInt = 0;
//
//				float startX = xInt * 0.25f;
//				float endX = xInt * 0.25f + 0.25f;
//				float startY = yInt * 0.25f;
//				float endY = yInt * 0.25f + 0.25f;
//
//				
//				uv[i*4+j*4*fogSizeX]     =new Vector2(endX-0.01f,endY-0.01f);
//				uv[i*4+1+j*4*fogSizeX]   =new Vector2(endX-0.01f,startY);
//				uv[i*4+2+j*4*fogSizeX]   =new Vector2(startX,endY-0.01f);
//				uv[i*4+3+j*4*fogSizeX]   =new Vector2(startX,startY);
//				
//				
//				//				uv[i*4+j*4*size]     =new Vector2(0.249f,0.249f);
//				//				uv[i*4+1+j*4*size]   =new Vector2(0.249f,0);
//				//				uv[i*4+2+j*4*size]   =new Vector2(0,0.249f);
//				//				uv[i*4+3+j*4*size]   =new Vector2(0,0);
//			}
//		}
//		
//		int[] triangles= new int[fogSizeX*fogSizeY*6];
//		for (int i = 0; i < fogSizeX; i++){
//			for (int j = 0; j < fogSizeY; j++){
//				triangles[i*6+j*6*fogSizeX]      =0+i*4+j*4*fogSizeX;
//				triangles[i*6+1+j*6*fogSizeX]    =1+i*4+j*4*fogSizeX;
//				triangles[i*6+2+j*6*fogSizeX]    =2+i*4+j*4*fogSizeX;
//				triangles[i*6+3+j*6*fogSizeX]    =2+i*4+j*4*fogSizeX;
//				triangles[i*6+4+j*6*fogSizeX]    =1+i*4+j*4*fogSizeX;
//				triangles[i*6+5+j*6*fogSizeX]    =3+i*4+j*4*fogSizeX;
//			}
//		}
//
//
//		Color[] colors = new Color[fogSizeX*fogSizeY*4];
//		
//		for (int i = 0; i < fogSizeX-1; i++){
//			for (int j = 0; j < fogSizeY-1; j++){
//
//				float alpha1 = 1 - fogMap[i+1,j+1]/2f;
//				float alpha2 = 1 - fogMap[i+1,j]/2f;
//				float alpha3 = 1 - fogMap[i,j+1]/2f;
//				float alpha4 = 1 - fogMap[i,j]/2f;
//
//
//				
//				colors[i*4+j*4*fogSizeX]     =new Color(1, 1, 1, alpha1);
//				colors[i*4+1+j*4*fogSizeX]   =new Color(1, 1, 1, alpha2);
//				colors[i*4+2+j*4*fogSizeX]   =new Color(1, 1, 1, alpha3);
//				colors[i*4+3+j*4*fogSizeX]   =new Color(1, 1, 1, alpha4);
//				
//				
//				//				uv[i*4+j*4*size]     =new Vector2(0.249f,0.249f);
//				//				uv[i*4+1+j*4*size]   =new Vector2(0.249f,0);
//				//				uv[i*4+2+j*4*size]   =new Vector2(0,0.249f);
//				//				uv[i*4+3+j*4*size]   =new Vector2(0,0);
//			}
//		}
//
//
//
//		mesh.Clear();
//		mesh.vertices = vertices;
//		mesh.uv = uv;
//		mesh.triangles = triangles;
//		mesh.colors = colors;
//		mesh.RecalculateBounds();
//		
//		GetComponent<MeshFilter>().mesh = mesh;
//	}
}
