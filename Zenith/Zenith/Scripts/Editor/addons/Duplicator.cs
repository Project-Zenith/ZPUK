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
public class Duplicator : EditorWindow
{
	public bool bHideWarning=false;
	public GameObject SelectedObject,ParentObject;
	public int NumberOfCopies=1;
	public bool bKeepParentObject=true,bCustomPosition;
	public struct OffsetStruct
	{
		public Vector3 V;
		public int n;

		public static OffsetStruct Create(Vector3 V, int n)
		{
			OffsetStruct OS=new OffsetStruct();
			OS.V=V;
			OS.n=n;
			return OS;
		}
	}
	public List<OffsetStruct> Offsets=new List<OffsetStruct>();
	public Vector3 StartPosition=new Vector3(),OffsetByIndex=new Vector3(-1,0,0),OffsetByRow=new Vector3(0,0,-1),OffsetByReset=new Vector3(-11,0,0),OffsetByReset2=new Vector3(0,3,0);
	public string UsedName="CustomDuplicateObject_%Index%",UsedTag="CreatedAutomaticalyDuplicateObject",CustomMessage;
	public int LengthOfRow=10,MaxRows=0,MaxResets=0;
	public bool bWorldPositionStays=true;
	public bool bUseFullSearch;
	Vector2 MainScroll=Vector2.zero;

	[MenuItem("Zenith/Recovery/Duplicator")]
	static void Init()
	{
		Duplicator window=(Duplicator)EditorWindow.GetWindow(typeof(Duplicator));
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

	public static List<GameObject> GetAllParents(GameObject GO)
	{
		if (!GO)
		{
			return new List<GameObject>();
		}

		return GetAllParents(GO.transform);
	}

	public static List<GameObject> GetAllParents(Transform T)
	{
		if (!T)
		{
			return new List<GameObject>();
		}

		List<GameObject> GO = new List<GameObject>();

		if (T.parent)
		{
			GO.Add(T.parent.gameObject);
			GO.AddRange(GetAllParents(T.parent));
		}

		return GO;
	}

	public static List<Transform> GetAllParentsT(GameObject GO)
	{
		if (!GO)
		{
			return new List<Transform>();
		}

		return GetAllParentsT(GO.transform);
	}

	public static List<Transform> GetAllParentsT(Transform T)
	{
		if (!T)
		{
			return new List<Transform>();
		}

		List<Transform> T2 = new List<Transform>();

		if (T.parent)
		{
			T2.Add(T.parent);
			T2.AddRange(GetAllParentsT(T.parent));
		}

		return T2;
	}

	public void OnGUI()
	{
		MainScroll=EditorGUILayout.BeginScrollView(MainScroll);

		if(!bHideWarning)
		{
			EditorGUILayout.LabelField("Warning! Don't Forget To Create Tag Before Using This Script");
			EditorGUILayout.LabelField("How To Do That:");
			EditorGUILayout.LabelField("1. Press On Any Object");
			EditorGUILayout.LabelField("2. Press On Tag");
			EditorGUILayout.LabelField("3. Press \"Add Tag...\"");
			EditorGUILayout.LabelField("4. Press \"+\"");
			EditorGUILayout.LabelField("5. Enter Tag Name And Press Save");
		}

		bHideWarning=EditorGUILayout.Toggle("Hide Warning", bHideWarning);
		SelectedObject=(GameObject)EditorGUILayout.ObjectField("Object", SelectedObject, typeof(GameObject), true);
		EditorGUILayout.BeginHorizontal();
		bKeepParentObject=EditorGUILayout.Toggle("Keep Parent Object", bKeepParentObject);

		if(!bKeepParentObject)
		{
			ParentObject=(GameObject)EditorGUILayout.ObjectField("Parent Object", ParentObject, typeof(GameObject), true);
		}

		EditorGUILayout.EndHorizontal();
		UsedName=EditorGUILayout.TextField("Used Name", UsedName);
		UsedTag=EditorGUILayout.TextField("Used Tag", UsedTag);
		NumberOfCopies=EditorGUILayout.IntField("Number Of Copies", NumberOfCopies);
		bCustomPosition=EditorGUILayout.Toggle("Custom Position", bCustomPosition);

		if(bCustomPosition)
		{
			EditorGUILayout.BeginHorizontal();
			StartPosition.x=EditorGUILayout.FloatField("StartPosition.X", StartPosition.x);
			StartPosition.y=EditorGUILayout.FloatField("StartPosition.Y", StartPosition.y);
			StartPosition.z=EditorGUILayout.FloatField("StartPosition.Z", StartPosition.z);
			EditorGUILayout.EndHorizontal();
			int l=EditorGUILayout.IntField("Offsets Length", Offsets.Count);

			while(l<Offsets.Count)
			{
				Offsets.RemoveAt(Offsets.Count-1);
			}

			while(l>Offsets.Count)
			{
				Offsets.Add(OffsetStruct.Create(new Vector3(), 0));
			}

			for(int i=0;i<Offsets.Count;i++)
			{
				OffsetStruct OS=Offsets[i];
				Vector3 V=OS.V;
				EditorGUILayout.BeginHorizontal();
				V.x=EditorGUILayout.FloatField("Offsets["+i+"].V.X", V.x);
				V.y=EditorGUILayout.FloatField("Offsets["+i+"].V.Y", V.y);
				V.z=EditorGUILayout.FloatField("Offsets["+i+"].V.Z", V.z);
				OS.V=V;
				OS.n=EditorGUILayout.IntField("Offsets["+i+"].n", OS.n);
				EditorGUILayout.EndHorizontal();
				Offsets[i]=OS;
			}

			if(bKeepParentObject || (ParentObject!=null && ParentObject.transform!=null))
			{
				bWorldPositionStays=EditorGUILayout.Toggle("World Position Stays", bWorldPositionStays);
			}
		}

		if(GUILayout.Button("Duplicate Object"))
		{
			DuplicateObject();
		}

		bUseFullSearch=EditorGUILayout.Toggle("Use Full Search", bUseFullSearch);

		if(GUILayout.Button("Destroy Objects By Tag"))
		{
			DestroyObjectsByTag();
		}

		if(CustomMessage!="")
		{
			EditorGUILayout.LabelField(CustomMessage);
		}

		EditorGUILayout.EndScrollView();
	}

	public void DuplicateObject()
	{
		GameObject MyObject,MyParentObject=null;
		int i,CurrentRow=0,CurrentReset=0,CurrentReset2=0;
		List<int> n=new List<int>();

		if(SelectedObject==null)
		{
			CustomMessage="You've To Select Object";
			return;
		}

		if(UsedName=="" || UsedTag=="")
		{
			CustomMessage="You've To Set Used Name And Used Tag";
			return;
		}

		CustomMessage="";
		List<int> m=new List<int>();

		for(i=0;i<NumberOfCopies;i++)
		{
			if(bCustomPosition)
			{
				if(i>0 && Offsets.Count>0)
				{
					while(m.Count<1)
					{
						m.Add(0);
					}

					m[0]++;

					for(int l=1;l<Offsets.Count;l++)
					{
						int p=Offsets[0].n;

						for(int o=1;o<l;o++)
						{
							p*=Offsets[o].n;
						}

						if(p>0 && i%p==0)
						{
							while(m.Count<l+1)
							{
								m.Add(0);
							}

							if(l>0)
							{
								m[l-1]=0;
							}

							m[l]++;
							continue;
						}

						break;
					}
				}

				Vector3 V2=new Vector3(0,0,0);

				for(int l=0;l<m.Count;l++)
				{
					if(l<Offsets.Count)
					{
						V2+=Offsets[l].V*m[l];
					}
				}

				Vector3 V=StartPosition+V2;
				MyObject=Instantiate(SelectedObject, V, SelectedObject.transform.rotation);
			}
			else
			{
				MyObject=Instantiate(SelectedObject);
			}

			MyObject.name=Repl(UsedName,"%Index%",i.ToString());
			MyObject.tag=UsedTag;

			if(bKeepParentObject)
			{
				if(SelectedObject.transform.parent!=null)
				{
					MyParentObject=SelectedObject.transform.parent.gameObject;
				}
			}
			else
			{
				MyParentObject=ParentObject;
			}

			if(MyParentObject!=null && MyParentObject.transform!=null)
			{
				if(bCustomPosition)
				{
					if(bWorldPositionStays)
					{
						List<Transform> T=GetAllParentsT(SelectedObject.transform);

						foreach(Transform T2 in T)
						{
							Vector3 V=MyObject.transform.localScale;
							V.x*=T2.localScale.x;
							V.y*=T2.localScale.y;
							V.z*=T2.localScale.z;
							MyObject.transform.localScale=V;
						}

						T.Clear();
					}

					MyObject.transform.SetParent(MyParentObject.transform,bWorldPositionStays);
				}
				else
				{
					MyObject.transform.SetParent(MyParentObject.transform,false);
				}
			}
		}
	}

	public GameObject[] FindChildrenObjectsByTag(GameObject GO, string S)
	{
		List<GameObject> Result=new List<GameObject>();
		GameObject[] FoundChildrenObjects;
		GameObject CurrentObject;

		for(int i=0; i<GO.transform.childCount; i++)
		{
			CurrentObject=GO.transform.GetChild(i).gameObject;

			if(CurrentObject.tag==S)
			{
				Result.Add(CurrentObject);
			}
			else
			{
				FoundChildrenObjects=FindChildrenObjectsByTag(CurrentObject,S);

				foreach(GameObject FoundChildrenObject in FoundChildrenObjects)
				{
					Result.Add(FoundChildrenObject);
				}
			}
		}

		return Result.ToArray();
	}

	public GameObject[] FindSceneGameObjectsByTag(string S, bool bFullSearch)
	{
		List<GameObject> Result=new List<GameObject>();
		Scene MyScene=SceneManager.GetActiveScene();
		List<GameObject> FoundObjects=new List<GameObject>();
		GameObject[] FoundChildrenObjects;
		MyScene.GetRootGameObjects(FoundObjects);

		foreach(GameObject FoundObject in FoundObjects)
		{
			if(FoundObject.tag==S)
			{
				Result.Add(FoundObject);
			}
			else if(bFullSearch)
			{
				FoundChildrenObjects=FindChildrenObjectsByTag(FoundObject,S);

				foreach(GameObject FoundChildrenObject in FoundChildrenObjects)
				{
					Result.Add(FoundChildrenObject);
				}
			}
		}

		return Result.ToArray();
	}

	public void DestroyObjectsByTag()
	{
		GameObject[] FoundObjects;

		if(UsedTag=="")
		{
			CustomMessage="You've To Set Used Tag";
			return;
		}

		CustomMessage="";
		FoundObjects=FindSceneGameObjectsByTag(UsedTag,bUseFullSearch);

		foreach(GameObject FoundObject in FoundObjects)
		{
			if(FoundObject!=SelectedObject)
			{
				DestroyObject(FoundObject);
			}
		}
	}

	public bool DestroyObject(GameObject MyGameObject)
	{
		if(MyGameObject==null)
		{
			return false;
		}

		DestroyImmediate(MyGameObject);
		return true;
	}
}
#endif
