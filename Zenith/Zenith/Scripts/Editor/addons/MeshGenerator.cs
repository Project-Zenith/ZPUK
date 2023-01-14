using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
#endif

using System;
using System.Linq;
//using VRC.Core;
//using Toolkit2;

#if UNITY_EDITOR
public class MeshGenerator : EditorWindow
{
	public GameObject SelectedObject;
	public string CustomMessage="";
	Vector2 MainScroll=Vector2.zero;
	public List<bool> GUIList_Bool=new List<bool>();

	[MenuItem("Zenith/Recovery/Mesh Generator")]
	static void Init()
	{
		MeshGenerator window=(MeshGenerator)EditorWindow.GetWindow(typeof(MeshGenerator));
		window.Show();
	}

	IEnumerator Sleep(double f)
	{
		while(f>0)
		{
			f-=Time.deltaTime;
			yield return null;
		}
	}

	string Eval(bool a, string b, string c)
	{
		if(a)
		{
			return b;
		}

		return c;
	}

	int Min(int i, int l)
	{
		if(i>l)
		{
			return l;
		}

		return i;
	}

	int Max(int i, int l)
	{
		if(i>l)
		{
			return i;
		}

		return l;
	}

	int Len(string S)
	{
		return S.Length;
	}

	public string Left(string S, int i)
	{
		if (S.Length <= i)
		{
			return S;
		}

		return S.Substring(0, i);
	}

	public string Right(string S, int i)
	{
		if (i >= S.Length)
		{
			return S;
		}

		return S.Substring(S.Length - i);
	}

	public string Mid(string S, int i, int l = 0)
	{
		if (l > 0)
		{
			return S.Substring(i, l);
		}

		return S.Substring(i);
	}

	int InStr(string Text, string TextB)
	{
		if(Text=="" || TextB=="")
		{
			return -1;
		}

		int i=0;
		int l=0;

		while(i<Len(Text))
		{
			if(Text[i]==TextB[l])
			{
				if(l==Len(TextB)-1)
				{
					return i-l;
				}

				l++;
			}
			else
			{
				l=0;
			}

			i++;
		}

		return -1;
	}

	List<int> InStrList(string Text, string TextB)
	{
		List<int> m=new List<int>();

		if(Text=="" || TextB=="")
		{
			return m;
		}

		int i=0;
		int l=0;

		while(i<Len(Text))
		{
			if(Text[i]==TextB[l])
			{
				l++;

				if(l==Len(TextB))
				{
					m.Add(i-l);
					l=0;
				}
			}
			else
			{
				l=0;
			}

			i++;
		}

		return m;
	}

	string Repl(string Text, string Replace, string With)
	{
		int i;
		string Input;

		if(Text=="" || Replace=="")
		{
			return Text;
		}

		Input=Text;
		Text="";
		i=InStr(Input, Replace);

		while(i!=-1)
		{
			Text=Text+Left(Input, i)+With;
			Input=Mid(Input, i+Len(Replace));
			i=InStr(Input, Replace);
		}

		return Text=Text+Input;
	}

	public void GetListOfMessages(string S, string DividerSymbol, out List<string> ListS)
	{
		List<int> i;
		string SS;
		int n,m=0;

		ListS=new List<string>();
		ListS.Clear();

		if(S!="")
		{
			i=InStrList(S,DividerSymbol);

			if(i==null || i.Count==0)
			{
				ListS.Add(S);
				return;
			}

			for(n=0;n<i.Count;n++)
			{
				SS="";

				while(m<=i[n])
				{
					SS+=S[m];
					m++;
				}

				if(SS!="")
				{
					ListS.Add(SS);
				}

				m=i[n]+Len(DividerSymbol)+1;
			}

			SS="";

			while(m<Len(S))
			{
				SS+=S[m];
				m++;
			}

			if(SS!="")
			{
				ListS.Add(SS);
			}
		}
	}

	void AddInfo(ref string S, string SS, string SSS)
	{
		if(S==null)
		{
			S="";
		}

		S=Eval(S=="",SS,S+SSS+SS);
	}

	void ShowLayersOfMessage(string S, string SS)
	{
		List<string> ListS;
		int i,l=0,bi=0;
		List<bool> b=new List<bool>();

		GetListOfMessages(S,SS,out ListS);

		for(i=0;i<ListS.Count;i++)
		{
			if(Left(ListS[i],Len("Foldout_Start("))=="Foldout_Start(" && Right(ListS[i],1)==")")
			{
				if(bi>=GUIList_Bool.Count)
				{
					GUIList_Bool.Add(true);
				}

				if(l==0)
				{
					GUIList_Bool[bi]=EditorGUILayout.Foldout(GUIList_Bool[bi], Mid(ListS[i],Len("Foldout_Start("),Len(ListS[i])-Len("Foldout_Start(")-1));
					b.Add(GUIList_Bool[bi]);
				}

				bi++;

				if(l==0 && b.Count>0 && b[b.Count-1])
				{
				}
				else
				{
					l++;
				}

				continue;
			}
			else if(ListS[i]=="Foldout_End")
			{
				if(!b[b.Count-1])
				{
					l--;
				}

				if(l==0)
				{
					b.RemoveAt(b.Count-1);
				}

				continue;
			}

			if(l==0 && (b.Count<=0 || b[b.Count-1]))
			{
				EditorGUILayout.LabelField(ListS[i]);
			}
		}
	}

	public string GetFullPathToObject(GameObject GO, GameObject GO2 = null)
	{
		if (!GO)
		{
			return "";
		}

		string S = "";
		GameObject GO_Parent = null;

		if (GO.transform.parent)
		{
			GO_Parent = GO.transform.parent.gameObject;
		}

		if (GO2)
		{
			while (GO_Parent && GO_Parent != GO2)
			{
				S = GO_Parent.name + "/" + S;

				if (GO_Parent.transform.parent)
				{
					GO_Parent = GO_Parent.transform.parent.gameObject;
				}
				else
				{
					GO_Parent = null;
				}
			}
		}
		else
		{
			while (GO_Parent)
			{
				S = GO_Parent.name + "/" + S;

				if (GO_Parent.transform.parent)
				{
					GO_Parent = GO_Parent.transform.parent.gameObject;
				}
				else
				{
					GO_Parent = null;
				}
			}
		}

		return S + GO.name;
	}

	public List<GameObject> FindChildrenObjectsOnce(GameObject GO)
	{
		List<GameObject> Result = new List<GameObject>();
		Transform T = GO.transform;

		for (int i = 0; i < T.childCount; i++)
		{
			GameObject CurrentObject = T.GetChild(i).gameObject;
			Result.Add(CurrentObject);
		}

		return Result;
	}

	public List<GameObject> FindChildrenObjects(GameObject GO)
	{
		List<GameObject> Result = new List<GameObject>();
		Transform T = GO.transform;

		for (int i = 0; i < T.childCount; i++)
		{
			GameObject CurrentObject = T.GetChild(i).gameObject;
			Result.Add(CurrentObject);
			List<GameObject> FoundChildrenObjects = FindChildrenObjects(CurrentObject);

			foreach (GameObject FoundChildrenObject in FoundChildrenObjects)
			{
				Result.Add(FoundChildrenObject);
			}

			FoundChildrenObjects.Clear();
		}

		return Result;
	}

	public List<GameObject> FindGameObjects(GameObject GO)
	{
		List<GameObject> Result = new List<GameObject>();
		Result.Add(GO);
		List<GameObject> FoundChildrenObjects = FindChildrenObjects(GO);

		foreach (GameObject FoundChildrenObject in FoundChildrenObjects)
		{
			Result.Add(FoundChildrenObject);
		}

		FoundChildrenObjects.Clear();
		return Result;
	}

	public struct TriangleIndices
	{
		public int v1;
		public int v2;
		public int v3;

		public TriangleIndices(int v1, int v2, int v3)
		{
			this.v1 = v1;
			this.v2 = v2;
			this.v3 = v3;
		}
	}

	public static int Sphere_getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
	{
		// first check if we have it already
		bool firstIsSmaller = p1 < p2;
		long smallerIndex = firstIsSmaller ? p1 : p2;
		long greaterIndex = firstIsSmaller ? p2 : p1;
		long key = (smallerIndex << 32) + greaterIndex;

		int ret;
		if (cache.TryGetValue(key, out ret))
		{
			return ret;
		}

		// not in cache, calculate it
		Vector3 point1 = vertices[p1];
		Vector3 point2 = vertices[p2];
		Vector3 middle = new Vector3
		(
			(point1.x + point2.x) / 2f,
			(point1.y + point2.y) / 2f,
			(point1.z + point2.z) / 2f
		);

		// add vertex makes sure point is on unit sphere
		int i = vertices.Count;
		vertices.Add(middle.normalized * radius);

		// store it, return index
		cache.Add(key, i);

		return i;
	}

	public void Sphere_CreateSphere(GameObject gameObject)
	{
		MeshFilter filter = gameObject.GetComponent<MeshFilter>();
		Mesh mesh = filter.mesh;
		mesh.Clear();
		Vector3[] vertices = gameObject.GetComponent<MeshFilter>().mesh.vertices;
		List<Vector3> vertList = new List<Vector3>();
		Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

		// create 12 vertices of a icosahedron
		float t = (1f + Mathf.Sqrt(5f)) / 2f;

		vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
		vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
		vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
		vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

		vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
		vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
		vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
		vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

		vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
		vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
		vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
		vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);

		// create 20 triangles of the icosahedron
		List<TriangleIndices> faces = new List<TriangleIndices>();

		// 5 faces around point 0
		faces.Add(new TriangleIndices(0, 11, 5));
		faces.Add(new TriangleIndices(0, 5, 1));
		faces.Add(new TriangleIndices(0, 1, 7));
		faces.Add(new TriangleIndices(0, 7, 10));
		faces.Add(new TriangleIndices(0, 10, 11));

		// 5 adjacent faces 
		faces.Add(new TriangleIndices(1, 5, 9));
		faces.Add(new TriangleIndices(5, 11, 4));
		faces.Add(new TriangleIndices(11, 10, 2));
		faces.Add(new TriangleIndices(10, 7, 6));
		faces.Add(new TriangleIndices(7, 1, 8));

		// 5 faces around point 3
		faces.Add(new TriangleIndices(3, 9, 4));
		faces.Add(new TriangleIndices(3, 4, 2));
		faces.Add(new TriangleIndices(3, 2, 6));
		faces.Add(new TriangleIndices(3, 6, 8));
		faces.Add(new TriangleIndices(3, 8, 9));

		// 5 adjacent faces 
		faces.Add(new TriangleIndices(4, 9, 5));
		faces.Add(new TriangleIndices(2, 4, 11));
		faces.Add(new TriangleIndices(6, 2, 10));
		faces.Add(new TriangleIndices(8, 6, 7));
		faces.Add(new TriangleIndices(9, 8, 1));

		// refine triangles
		for (int i = 0; i < recursionLevel; i++)
		{
			List<TriangleIndices> faces2 = new List<TriangleIndices>();
			foreach (var tri in faces)
			{
				// replace triangle by 4 triangles
				int a = Sphere_getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
				int b = Sphere_getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
				int c = Sphere_getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

				faces2.Add(new TriangleIndices(tri.v1, a, c));
				faces2.Add(new TriangleIndices(tri.v2, b, a));
				faces2.Add(new TriangleIndices(tri.v3, c, b));
				faces2.Add(new TriangleIndices(a, b, c));
			}
			faces = faces2;
		}

		mesh.vertices = vertList.ToArray();

		List<int> triList = new List<int>();
		for (int i = 0; i < faces.Count; i++)
		{
			triList.Add(faces[i].v1);
			triList.Add(faces[i].v2);
			triList.Add(faces[i].v3);
		}
		mesh.triangles = triList.ToArray();
		mesh.uv = new Vector2[vertices.Length];

		Vector3[] normales = new Vector3[vertList.Count];
		for (int i = 0; i < normales.Length; i++)
			normales[i] = vertList[i].normalized;


		mesh.normals = normales;

		mesh.RecalculateBounds();
		mesh.RecalculateTangents();
		mesh.RecalculateNormals();
		//mesh.Optimize();
	}

	public Material planetMaterial = null;
	public float planetSize = 1f;
	GameObject planet;
	Mesh planetMesh;
	Vector3[] planetVertices;
	int[] planetTriangles;
	MeshRenderer planetMeshRenderer;
	MeshFilter planetMeshFilter;
	MeshCollider planetMeshCollider;

	int recursionLevel = 3;
	float radius = 1f;

	public void Sphere_CreatePlanet(GameObject GO)
	{
		Sphere_CreatePlanetGameObject(GO);
		//do whatever else you need to do with the sphere mesh
		Sphere_RecalculateMesh();
	}

	public void Sphere_CreatePlanetGameObject(GameObject GO=null)
	{
		if(!GO)
		{
			planet = new GameObject();
			planetMeshFilter = planet.AddComponent<MeshFilter>();
			planetMesh = planetMeshFilter.mesh;
			planetMeshRenderer = planet.AddComponent<MeshRenderer>();
			//need to set the material up top
			planetMeshRenderer.material = planetMaterial;
			planet.transform.localScale = new Vector3(planetSize, planetSize, planetSize);
		}
		else
		{
			planet=GO;
		}

		Sphere_CreateSphere(planet);
	}

	public void Sphere_RecalculateMesh()
	{
		planetMesh.RecalculateBounds();
		planetMesh.RecalculateTangents();
		planetMesh.RecalculateNormals();
	}

	public void Cylinder_MakingVertices(int radius, int iterations, int lenggth, float gap, float noise)
	{
		float noise_x;
		float noise_y;
		float noise_z;
		float x;
		float y;
		float z = 0;
		int i;
		int p = 0;
		float angle;
 
		vertices = new Vector3[(iterations * lenggth) + 2];
		int tempo = 0;
		vertices[vertices.Length - 2] = Vector3.zero;
		num=0;
 
		while (p < lenggth)
		{
			i = 0;
			while (i < iterations)
			{
				angle = (i * 1.0f) / iterations * Mathf.PI * 2;
				x = Mathf.Sin(angle) * radius;
				y = Mathf.Cos(angle) * radius;
				vertices[tempo] = new Vector3(x, y, z);
				//GameObject go = Instantiate(cube, vertices[tempo], Quaternion.identity);
				//go.name = num.ToString();
				i++;
				num++;
				tempo += 1;
			}
			z += gap;
			p++;
		}
 
 
		vertices[vertices.Length - 1] = new Vector3(0, 0, vertices[vertices.Length - 3].z);
		Debug.Log("Vertices: " + num);
		Cylinder_Mesh.vertices = vertices;
		Cylinder_MakingNormals();
	}

	public void Cylinder_MakingNormals()
	{
		int i = 0;
		//Vector3[] normals = new Vector3[num + 2];
		Vector3[] normals = new Vector3[Cylinder_Mesh.vertices.Length];
		while (i < Cylinder_Mesh.vertices.Length/*num*/)
		{
			normals[i] = Vector3.forward;
			i++;
		}
		Cylinder_Mesh.normals = normals;
 
		Cylinder_MakingTrianges();
	}

	public void Cylinder_MakingTrianges()
	{
		int i = 0;
		tris = new int[((3 * (leng - 1) * iter) * 2) + 3];
		while (i < (leng - 1) * iter)
		{
			tris[i * 3] = i;
			if ((i + 1) % iter == 0)
			{
				tris[i * 3 + 1] = 1 + i - iter;
			}
			else
			{
				tris[i * 3 + 1] = 1 + i;
			}
			tris[i * 3 + 2] = iter + i;
			i++;
		}
		int IndexofNewTriangles = -1;
 
		for (int u = (tris.Length - 3) / 2; u < tris.Length - 6; u += 3)
		{
			//Cylinder_Mesh.RecalculateTangents();
			if ((IndexofNewTriangles + 2) % iter == 0)
			{
				tris[u] = IndexofNewTriangles + iter * 2 + 1;
			}
			else
				tris[u] = IndexofNewTriangles + iter + 1;
 
			tris[u + 1] = IndexofNewTriangles + 2;
			tris[u + 2] = IndexofNewTriangles + iter + 2;
			IndexofNewTriangles += 1;
		}
		tris[tris.Length - 3] = 0;
		tris[tris.Length - 2] = (iter * 2) - 1;
		tris[tris.Length - 1] = iter;
 
		firstplane = new int[(iter * 3) * 2];
		int felmnt = 0;
		for (int h = 0; h < firstplane.Length / 2; h += 3)
		{
 
			firstplane[h] = felmnt;
 
			if (felmnt + 1 != iter)
				firstplane[h + 1] = felmnt + 1;
			else
				firstplane[h + 1] = 0;
			firstplane[h + 2] = vertices.Length - 2;
			felmnt += 1;
		}
 
		felmnt = iter * (leng - 1);
		for (int h = firstplane.Length / 2; h < firstplane.Length; h += 3)
		{
 
			firstplane[h] = felmnt;
 
			if (felmnt + 1 != iter * (leng - 1))
				firstplane[h + 1] = felmnt + 1;
			else
				firstplane[h + 1] = iter * (leng - 1);
			firstplane[h + 2] = vertices.Length - 1;
			felmnt += 1;
		}
 
		firstplane[firstplane.Length - 3] = iter * (leng - 1);
		firstplane[firstplane.Length - 2] = vertices.Length - 3;
		firstplane[firstplane.Length - 1] = vertices.Length - 1;
 
		FinalTri = new int[tris.Length + firstplane.Length];
 
		int k = 0, l = 0;
		for (k = 0, l = 0; k < tris.Length; k++)
		{
			FinalTri[l++] = tris[k];
		}
		for (k = 0; k < firstplane.Length; k++)
		{
			FinalTri[l++] = firstplane[k];
		}
 
		Cylinder_Mesh.triangles = FinalTri;
		//Cylinder_Mesh.Optimize();
		Cylinder_Mesh.RecalculateNormals();
		Cylinder_MeshFilter.mesh = Cylinder_Mesh;
	}

	public MeshFilter Cylinder_MeshFilter;
	public Mesh Cylinder_Mesh;
	public int iter;
	int num;
	public int leng;
	Vector3[] vertices;
	int[] tris;
	int[] FinalTri;
	int[] firstplane;

	public void CrashSphere_CreateSphere(GameObject gameObject)
	{
		MeshFilter filter = gameObject.GetComponent<MeshFilter>();
		Mesh mesh = filter.mesh;
		mesh.Clear();
		Vector3[] vertices = gameObject.GetComponent<MeshFilter>().mesh.vertices;
		List<Vector3> vertList = new List<Vector3>();
		Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

		// create 12 vertices of a icosahedron
		float t = (1f + Mathf.Sqrt(5f)) / 2f;

		vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
		vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
		vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
		vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

		vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
		vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
		vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
		vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

		vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
		vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
		vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
		vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);

		// create 20 triangles of the icosahedron
		List<TriangleIndices> faces = new List<TriangleIndices>();

		// 5 faces around point 0
		faces.Add(new TriangleIndices(0, 11, 5));
		faces.Add(new TriangleIndices(0, 5, 1));
		faces.Add(new TriangleIndices(0, 1, 7));
		faces.Add(new TriangleIndices(0, 7, 10));
		faces.Add(new TriangleIndices(0, 10, 11));

		// 5 adjacent faces 
		faces.Add(new TriangleIndices(1, 5, 9));
		faces.Add(new TriangleIndices(5, 11, 4));
		faces.Add(new TriangleIndices(11, 10, 2));
		faces.Add(new TriangleIndices(10, 7, 6));
		faces.Add(new TriangleIndices(7, 1, 8));

		// 5 faces around point 3
		faces.Add(new TriangleIndices(3, 9, 4));
		faces.Add(new TriangleIndices(3, 4, 2));
		faces.Add(new TriangleIndices(3, 2, 6));
		faces.Add(new TriangleIndices(3, 6, 8));
		faces.Add(new TriangleIndices(3, 8, 9));

		// 5 adjacent faces 
		faces.Add(new TriangleIndices(4, 9, 5));
		faces.Add(new TriangleIndices(2, 4, 11));
		faces.Add(new TriangleIndices(6, 2, 10));
		faces.Add(new TriangleIndices(8, 6, 7));
		faces.Add(new TriangleIndices(9, 8, 1));

		// refine triangles
		for (int i = 0; i < recursionLevel; i++)
		{
			List<TriangleIndices> faces2 = new List<TriangleIndices>();
			foreach (var tri in faces)
			{
				// replace triangle by 4 triangles
				int a = Sphere_getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
				int b = Sphere_getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
				int c = Sphere_getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

				faces2.Add(new TriangleIndices(tri.v1, a, c));
				faces2.Add(new TriangleIndices(tri.v2, b, a));
				faces2.Add(new TriangleIndices(tri.v3, c, b));
				faces2.Add(new TriangleIndices(a, b, c));
			}
			faces = faces2;
		}

		mesh.vertices = vertList.ToArray();

		List<int> triList = new List<int>();
		/*for (int i = 0; i < faces.Count; i++)
		{
			triList.Add(faces[i].v1);
			triList.Add(faces[i].v2);
			triList.Add(faces[i].v3);
		}*/
		int vectCount=vertList.Count;
		for (int i = 0; i < vectCount; i++)
		{
			for (int l = i+1; l < vectCount; l++)
			{
				for (int m = l+1; m < vectCount; m++)
				{
					triList.Add(i);
					triList.Add(l);
					triList.Add(m);
				}
			}
		}
		mesh.triangles = triList.ToArray();
		mesh.uv = new Vector2[vertices.Length];

		Vector3[] normales = new Vector3[vertList.Count];
		for (int i = 0; i < normales.Length; i++)
			normales[i] = vertList[i].normalized;


		mesh.normals = normales;

		mesh.RecalculateBounds();
		mesh.RecalculateTangents();
		mesh.RecalculateNormals();
		//mesh.Optimize();
	}

	public void CrashSphere_CreatePlanet(GameObject GO)
	{
		CrashSphere_CreatePlanetGameObject(GO);
		//do whatever else you need to do with the sphere mesh
		Sphere_RecalculateMesh();
	}

	public void CrashSphere_CreatePlanetGameObject(GameObject GO=null)
	{
		if(!GO)
		{
			planet = new GameObject();
			planetMeshFilter = planet.AddComponent<MeshFilter>();
			planetMesh = planetMeshFilter.mesh;
			planetMeshRenderer = planet.AddComponent<MeshRenderer>();
			//need to set the material up top
			planetMeshRenderer.material = planetMaterial;
			planet.transform.localScale = new Vector3(planetSize, planetSize, planetSize);
		}
		else
		{
			planet=GO;
		}

		CrashSphere_CreateSphere(planet);
	}

    public Mesh CubeMesh;
	private List<Vector3> Cube_vertices;
	private List<int> Cube_triangles;
	public float CubeSize;

	public void Cube_CreateCube(GameObject GO)
	{
		CubeMesh.Clear();
		Cube_vertices.Clear();
		Cube_triangles.Clear();
		Transform transform=GO.transform;

		var x = (int) transform.position.x;
		var y = (int) transform.position.y;
		var z = (int) transform.position.z;

		var blockVertices = new Vector3[8];

		for (var i = 0; i < 8; i++)
		{
			var newX = x + ((i >> 2 & 1) * CubeSize)-CubeSize*0.5f;
			var newY = y + ((i >> 1 & 1) * CubeSize)-CubeSize*0.5f;
			var newZ = z + ((i >> 0 & 1) * CubeSize)-CubeSize*0.5f;
			blockVertices[i] = new Vector3(newX, newY, newZ);
		}

		for (var i = 0; i < 6; i++)
		{
			var sign = i % 2;
			var pos = i % 3;

			var vertices = new int[4];
			var a = (sign == 1 ? (1 << pos) : 0);

			vertices[0] = a + (0 << ((pos + 1) % 3)) + (0 << ((pos + 2) % 3));
			vertices[1] = a + (0 << ((pos + 1) % 3)) + (1 << ((pos + 2) % 3));
			vertices[2] = a + (1 << ((pos + 1) % 3)) + (0 << ((pos + 2) % 3));
			vertices[3] = a + (1 << ((pos + 1) % 3)) + (1 << ((pos + 2) % 3));

			Array.Sort(vertices);

			vertices[0] = Cube_AddVertex(blockVertices[vertices[0]]);
			vertices[1] = Cube_AddVertex(blockVertices[vertices[1]]);
			vertices[2] = Cube_AddVertex(blockVertices[vertices[2]]);
			vertices[3] = Cube_AddVertex(blockVertices[vertices[3]]);

			if (i < 3)
			{
				Cube_AddTriangle(vertices[0], vertices[1], vertices[3]);
				Cube_AddTriangle(vertices[0], vertices[3], vertices[2]);
			}
			else
			{
				Cube_AddTriangle(vertices[0], vertices[2], vertices[3]);
				Cube_AddTriangle(vertices[0], vertices[3], vertices[1]);
			}
		}

		CubeMesh.vertices = Cube_vertices.ToArray();
		CubeMesh.triangles = Cube_triangles.ToArray();
		CubeMesh.RecalculateNormals();
	}

	private int Cube_AddVertex(Vector3 value)
	{
		var vertexIndex = Cube_vertices.Count;
		Cube_vertices.Add(value);
		return vertexIndex;
	}

	private void Cube_AddTriangle(int v1, int v2, int v3)
	{
		Cube_triangles.Add(v1);
		Cube_triangles.Add(v2);
		Cube_triangles.Add(v3);
	}

	public enum EMeshType
	{
		MT_None,
		MT_Cube,
		MT_Sphere,
		MT_CrashSphere,
		MT_Cylinder
	}

	public EMeshType MeshType;

	public void OnGUI()
	{
		MainScroll=EditorGUILayout.BeginScrollView(MainScroll);
		SelectedObject=(GameObject)EditorGUILayout.ObjectField("Object", SelectedObject, typeof(GameObject), true);
		MeshType = (EMeshType)EditorGUILayout.EnumPopup("Primitive to create:", MeshType);

		if(MeshType==EMeshType.MT_Cube)
		{
			CubeSize=EditorGUILayout.FloatField("Size", CubeSize);

			if(GUILayout.Button("Generate Cube"))
			{
				GenerateCube(SelectedObject);
			}
		}
		else if(MeshType==EMeshType.MT_Sphere)
		{
			planetSize=EditorGUILayout.FloatField("Size", planetSize);
			recursionLevel=EditorGUILayout.IntField("Recursion Level", recursionLevel);
			radius=EditorGUILayout.FloatField("Radius", radius);

			if(GUILayout.Button("Generate Sphere"))
			{
				GenerateSphere(SelectedObject);
			}
		}
		else if(MeshType==EMeshType.MT_CrashSphere)
		{
			planetSize=EditorGUILayout.FloatField("Size", planetSize);
			recursionLevel=EditorGUILayout.IntField("Recursion Level", recursionLevel);
			radius=EditorGUILayout.FloatField("Radius", radius);

			if(GUILayout.Button("Generate Sphere"))
			{
				GenerateCrashSphere(SelectedObject);
			}
		}
		else if(MeshType==EMeshType.MT_Cylinder)
		{
			iter=EditorGUILayout.IntField("Iter", iter);
			leng=EditorGUILayout.IntField("Leng", leng);

			if(GUILayout.Button("Create Cylinder"))
			{
				GenerateCylinder(SelectedObject);
			}
		}

		if(CustomMessage!="")
		{
			ShowLayersOfMessage(CustomMessage,"|");
		}

		EditorGUILayout.EndScrollView();
	}

	public bool GenerateCube(GameObject GO)
	{
		MeshFilter CubeMeshFilter=null;

		if(!GO)
		{
			GO = new GameObject();
			CubeMeshFilter = GO.AddComponent<MeshFilter>();
			CubeMesh = CubeMeshFilter.mesh;
			MeshRenderer MyMeshRenderer = GO.AddComponent<MeshRenderer>();
			GO.transform.localScale = new Vector3(1, 1, 1);
		}
		else
		{
			CubeMeshFilter = GO.GetComponent<MeshFilter>();

			if(CubeMeshFilter)
			{
				CubeMesh=CubeMeshFilter.mesh;
			}
		}

		if(CubeMeshFilter && CubeMesh)
		{
			Cube_vertices = new List<Vector3>();
			Cube_triangles = new List<int>();
			Cube_CreateCube(GO);
		}

		CustomMessage="";
		return true;
	}

	public bool GenerateSphere(GameObject GO)
	{
		/*if(!GO)
		{
			CustomMessage="GameObject Is Null!";
			return false;
		}*/

		Sphere_CreatePlanet(GO);
		CustomMessage="";
		return true;
	}

	public bool GenerateCrashSphere(GameObject GO)
	{
		/*if(!GO)
		{
			CustomMessage="GameObject Is Null!";
			return false;
		}*/

		CrashSphere_CreatePlanet(GO);
		CustomMessage="";
		return true;
	}

	public bool GenerateCylinder(GameObject GO)
	{
		if(!GO)
		{
			GO = new GameObject();
			Cylinder_MeshFilter = GO.AddComponent<MeshFilter>();
			Cylinder_Mesh = Cylinder_MeshFilter.mesh;
			MeshRenderer MyMeshRenderer = GO.AddComponent<MeshRenderer>();
			//need to set the material up top
			MyMeshRenderer.material = planetMaterial;
			GO.transform.localScale = new Vector3(1, 1, 1);
		}
		else
		{
			Cylinder_MeshFilter = GO.GetComponent<MeshFilter>();

			if(Cylinder_MeshFilter)
			{
				Cylinder_Mesh=Cylinder_MeshFilter.mesh;
			}
		}

		if(Cylinder_MeshFilter && Cylinder_Mesh)
		{
			Cylinder_MakingVertices(1, iter, leng, 0.5f, 0.1f);
		}

		CustomMessage="";
		return true;
	}
}
#endif
