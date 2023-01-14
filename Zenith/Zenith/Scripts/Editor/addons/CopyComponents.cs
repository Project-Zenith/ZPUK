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
public class CopyComponents : EditorWindow
{
	public GameObject SelectedObject,CopyFromObject;
	public string CustomMessage="";
	Vector2 MainScroll=Vector2.zero;
	public List<bool> GUIList_Bool=new List<bool>();
	public bool bSkipMissingGameObjects=false;

	[MenuItem("Zenith/Recovery/Copy Components")]
	static void Init()
	{
		CopyComponents window=(CopyComponents)EditorWindow.GetWindow(typeof(CopyComponents));
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

	public List<string> Split(string Text, string TextB)
	{
		List<string> S2 = new List<string>();

		if (Text == "" || TextB == "")
		{
			S2.Add(Text);
			return S2;
		}

		int i = 0;
		int l = 0;
		int m = 0;

		while (i < Len(Text))
		{
			if (Text[i] == TextB[l])
			{
				if (l == Len(TextB) - 1)
				{
					S2.Add(Mid(Text, m, i - l - m));
					m = i + 1;
					l = 0;
				}
				else
				{
					l++;
				}
			}
			else
			{
				l = 0;
			}

			i++;
		}

		S2.Add(Mid(Text, m));
		return S2;
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

	public Transform FindIn(GameObject GO, string S)
	{
		return FindIn(GO.transform, S);
	}

	public Transform FindIn(Transform T, string S)
	{
		List<string> S2 = Split(S, "/");
		Transform T2 = FindIn(T, S2);
		S2.Clear();
		return T2;
	}

	public Transform FindIn(Transform T, List<string> S)
	{
		if (!T || S.Count == 0)
		{
			return null;
		}

		Transform T2 = null;

		if (!string.IsNullOrEmpty(S[0]))
		{
			for (int i = 0; i < T.childCount; i++)
			{
				Transform T3 = T.GetChild(i);

				if (T3.name == S[0])
				{
					T2 = T3;
					break;
				}
			}
		}

		if (S.Count == 1)
		{
			return T2;
		}

		S.RemoveAt(0);
		Transform T4 = FindIn(T2, S);
		S.Clear();
		return T4;
	}

	public void OnGUI()
	{
		MainScroll=EditorGUILayout.BeginScrollView(MainScroll);
		SelectedObject=(GameObject)EditorGUILayout.ObjectField("Object", SelectedObject, typeof(GameObject), true);
		CopyFromObject=(GameObject)EditorGUILayout.ObjectField("Copy From", CopyFromObject, typeof(GameObject), true);
		bSkipMissingGameObjects=EditorGUILayout.Toggle("Skip Missing GameObjects", bSkipMissingGameObjects);

		if(GUILayout.Button("Copy Missing Components"))
		{
			CopyMissingComponents(SelectedObject,CopyFromObject);
		}

		if(GUILayout.Button("Find Missing Components"))
		{
			FindMissingComponents(SelectedObject,CopyFromObject);
		}

		if(CustomMessage!="")
		{
			ShowLayersOfMessage(CustomMessage,"|");
		}

		EditorGUILayout.EndScrollView();
	}

	public bool CopyMissingComponents(GameObject GO, GameObject GO2)
	{
		if(!GO)
		{
			CustomMessage="GameObject Is Null!";
			return false;
		}

		if(!GO2)
		{
			CustomMessage="GameObject2 Is Null!";
			return false;
		}

		CustomMessage="";
		List<GameObject> GO3=FindGameObjects(GO2);

		foreach(GameObject GO4 in GO3)
		{
			string S=GetFullPathToObject(GO4,GO2);
			Transform T=FindIn(GO,S);

			if(!T)
			{
				if(!bSkipMissingGameObjects)
				{
					AddInfo(ref CustomMessage,S+" Is Missing!","|");
				}

				continue;
			}

			GameObject GO5=T.gameObject;

			if(!GO5)
			{
				AddInfo(ref CustomMessage,S+" GameObject Is Missing!","|");
				continue;
			}

			Component[] components = GO4.GetComponents<Component>();

			foreach (Component C in components)
			{
				if(C)
				{
					Component[] components2 = GO5.GetComponents<Component>();
					bool bMissing=true;

					foreach (Component C2 in components2)
					{
						if(C2)
						{
							if (C.ToString() == C2.ToString())
							{
								bMissing=false;
								break;
							}
						}
					}

					if(bMissing)
					{
						AddInfo(ref CustomMessage,GetFullPathToObject(GO4,GO2)+": Added Missed Component "+C.ToString(),"|");
					}
				}
			}
		}

		return true;
	}

	public bool FindMissingComponents(GameObject GO, GameObject GO2)
	{
		if(!GO)
		{
			CustomMessage="GameObject Is Null!";
			return false;
		}

		if(!GO2)
		{
			CustomMessage="GameObject2 Is Null!";
			return false;
		}

		CustomMessage="";
		List<GameObject> GO3=FindGameObjects(GO2);

		foreach(GameObject GO4 in GO3)
		{
			string S=GetFullPathToObject(GO4,GO2);
			Transform T=FindIn(GO,S);

			if(!T)
			{
				if(!bSkipMissingGameObjects)
				{
					AddInfo(ref CustomMessage,S+" Is Missing!","|");
				}

				continue;
			}

			GameObject GO5=T.gameObject;

			if(!GO5)
			{
				AddInfo(ref CustomMessage,S+" GameObject Is Missing!","|");
				continue;
			}

			Component[] components = GO4.GetComponents<Component>();

			foreach (Component C in components)
			{
				Component[] components2 = GO5.GetComponents<Component>();
				bool bMissing=true;

				foreach (Component C2 in components2)
				{
					if (C.ToString() == C2.ToString())
					{
						bMissing=false;
						break;
					}
				}

				if(bMissing)
				{
					AddInfo(ref CustomMessage,GetFullPathToObject(GO4,GO2)+": Missing Component "+C.ToString(),"|");
				}
			}
		}

		return true;
	}
}
#endif
