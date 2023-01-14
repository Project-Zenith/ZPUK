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
public class FindMaterial : EditorWindow
{
	public GameObject SelectedObject;
	public string MaterialName="";
	public string CustomMessage="";
	Vector2 MainScroll=Vector2.zero;
	public List<bool> GUIList_Bool=new List<bool>();

	[MenuItem("Zenith/Recovery/Find Material")]
	static void Init()
	{
		FindMaterial window=(FindMaterial)EditorWindow.GetWindow(typeof(FindMaterial));
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

	public void OnGUI()
	{
		MainScroll=EditorGUILayout.BeginScrollView(MainScroll);
		SelectedObject=(GameObject)EditorGUILayout.ObjectField("Object", SelectedObject, typeof(GameObject), true);
		MaterialName=EditorGUILayout.TextField("Material Name", MaterialName);

		if(GUILayout.Button("Find Material"))
		{
			FindMaterialInGO(SelectedObject);
		}

		if(CustomMessage!="")
		{
			ShowLayersOfMessage(CustomMessage,"|");
		}

		EditorGUILayout.EndScrollView();
	}

	public bool FindMaterialInGO(GameObject GO)
	{
		if(!GO)
		{
			CustomMessage="GameObject Is Null!";
			return false;
		}

		CustomMessage="";
		List<GameObject> GO2=FindGameObjects(GO);

		foreach(GameObject GO3 in GO2)
		{
			Renderer[] renderers = GO3.GetComponents<Renderer>();

			for (int i = 0; i < renderers.Length; i++)
			{
				if (renderers[i] != null)
				{
					Material[] M=renderers[i].sharedMaterials;

					for (int l = 0; l < M.Length; l++)
					{
						if(M[l]!=null)
						{
							string S=M[l].name;
							
							if(Right(S,Len(" (Instance)"))==" (Instance)")
							{
								S=Left(S,Len(S)-Len(" (Instance)"));
							}
							
							if(S==MaterialName)
							{
								AddInfo(ref CustomMessage,GetFullPathToObject(GO3,GO),"|");
							}
						}
					}
				}
			}
		}

		return true;
	}
}
#endif
