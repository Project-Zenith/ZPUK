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
//using Toolkit;

#if UNITY_EDITOR
public class MeshToAsset : EditorWindow
{
	public GameObject SelectedObject;
	public string OutputPath="Assets\\";
	public string CustomMessage;
	Vector2 MainScroll=Vector2.zero;
	public List<bool> GUIList_Bool=new List<bool>();

	[MenuItem("Zenith/Recovery/Convert Mesh To Asset")]
	static void Init()
	{
		MeshToAsset window=(MeshToAsset)EditorWindow.GetWindow(typeof(MeshToAsset));
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

	public void OnGUI()
	{
		MainScroll=EditorGUILayout.BeginScrollView(MainScroll);
		SelectedObject=(GameObject)EditorGUILayout.ObjectField("Object", SelectedObject, typeof(GameObject), true);
		OutputPath=EditorGUILayout.TextField("Output Path", OutputPath);

		if(GUILayout.Button("Save Asset"))
		{
			SaveAsset(SelectedObject);
		}

		if(CustomMessage!="")
		{
			EditorGUILayout.LabelField(CustomMessage);
		}

		EditorGUILayout.EndScrollView();
	}

	public bool SaveAsset(GameObject GO)
	{
		if(GO==null)
		{
			CustomMessage="You've To Select Object";
			return false;
		}

		MeshFilter MF=GO.GetComponent<MeshFilter>();

		if(MF==null)
		{
			CustomMessage="Selected Object Does Not Contain Mesh Filter";
			return false;
		}

		Mesh M=MF.mesh;

		if(M==null)
		{
			CustomMessage="Mesh Filter Does Not Contain Mesh";
			return false;
		}

		string S="";

		if(Right(OutputPath,Len(".asset"))==".asset")
		{
			S=OutputPath;
		}
		else
		{
			S=OutputPath+".asset";
		}

		CustomMessage="";
		AssetDatabase.CreateAsset(M, S);

		return true;
	}
}
#endif
