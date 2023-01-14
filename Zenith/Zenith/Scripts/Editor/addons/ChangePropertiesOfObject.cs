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
public class ChangePropertiesOfObject : EditorWindow
{
	public GameObject SelectedObject;
	public string CustomMessage;
	Vector2 MainScroll=Vector2.zero;

	public struct ObjectProperties_Struct
	{
		public GameObject GO;
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 Scale;

        public static ObjectProperties_Struct Create(GameObject GO, Vector3 Position, Quaternion Rotation, Vector3 Scale)
        {
            ObjectProperties_Struct OPS = new ObjectProperties_Struct();
            OPS.GO = GO;
            OPS.Position = Position;
            OPS.Rotation = Rotation;
            OPS.Scale = Scale;
            return OPS;
        }
	};

	List<ObjectProperties_Struct> ObjectProperties=new List<ObjectProperties_Struct>();
	Vector3 NewPosition=new Vector3(0,0,0);
	Quaternion NewRotation=new Quaternion(0,0,0,0);
	Vector3 NewScale=new Vector3(0,0,0);
	bool bChangePosition=false,bChangeRotation=false,bChangeScale=false;
	bool bKeepChildrenPosition=false,bKeepChildrenRotation=false,bKeepChildrenScale=false;

	[MenuItem("Zenith/Recovery/Change Properties Of Object")]
	static void Init()
	{
		ChangePropertiesOfObject window=(ChangePropertiesOfObject)EditorWindow.GetWindow(typeof(ChangePropertiesOfObject));
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
		bChangePosition=EditorGUILayout.Toggle("Change Position", bChangePosition);

		if(bChangePosition)
		{
			EditorGUILayout.BeginHorizontal();
			NewPosition.x=EditorGUILayout.FloatField("Position.X", NewPosition.x);
			NewPosition.y=EditorGUILayout.FloatField("Position.Y", NewPosition.y);
			NewPosition.z=EditorGUILayout.FloatField("Position.Z", NewPosition.z);
			EditorGUILayout.EndHorizontal();
		}

		bChangeRotation=EditorGUILayout.Toggle("Change Rotation", bChangeRotation);

		if(bChangeRotation)
		{
			EditorGUILayout.BeginHorizontal();
			NewRotation.x=EditorGUILayout.FloatField("Rotation.X", NewRotation.x);
			NewRotation.y=EditorGUILayout.FloatField("Rotation.Y", NewRotation.y);
			NewRotation.z=EditorGUILayout.FloatField("Rotation.Z", NewRotation.z);
			NewRotation.w=EditorGUILayout.FloatField("Rotation.W", NewRotation.w);
			EditorGUILayout.EndHorizontal();
		}

		bChangeScale=EditorGUILayout.Toggle("Change Scale", bChangeScale);

		if(bChangeScale)
		{
			EditorGUILayout.BeginHorizontal();
			NewScale.x=EditorGUILayout.FloatField("Scale.X", NewScale.x);
			NewScale.y=EditorGUILayout.FloatField("Scale.Y", NewScale.y);
			NewScale.z=EditorGUILayout.FloatField("Scale.Z", NewScale.z);
			EditorGUILayout.EndHorizontal();
		}

		bKeepChildrenPosition=EditorGUILayout.Toggle("Keep Children Position", bKeepChildrenPosition);
		bKeepChildrenRotation=EditorGUILayout.Toggle("Keep Children Rotation", bKeepChildrenRotation);
		bKeepChildrenScale=EditorGUILayout.Toggle("Keep Children Scale", bKeepChildrenScale);

		if(GUILayout.Button("Set Properties"))
		{
			SetProperties(SelectedObject);
		}

		if(CustomMessage!="")
		{
			EditorGUILayout.LabelField(CustomMessage);
		}

		EditorGUILayout.EndScrollView();
	}

	public bool SetProperties(GameObject GO)
	{
		if(GO==null)
		{
			CustomMessage="You've To Select Object";
			return false;
		}

		CustomMessage="";
		Transform MyTransform=GO.transform;

		if(MyTransform==null)
		{
			CustomMessage="Selected Object Does Not Contain Transform";
			return false;
		}

		List<GameObject> Children=FindChildrenObjects(GO);

		if(Children.Count>0)
		{
			foreach(GameObject GO2 in Children)
			{
				Transform T=GO2.transform;

				if(T==null)
				{
					continue;
				}

				ObjectProperties.Add(ObjectProperties_Struct.Create(GO2,T.position,T.rotation,T.lossyScale));
			}

			if(bChangePosition)
			{
				MyTransform.localPosition=NewPosition;
			}

			if(bChangeRotation)
			{
				MyTransform.localRotation=NewRotation;
			}

			if(bChangeScale)
			{
				MyTransform.localScale=NewScale;
			}

			foreach(ObjectProperties_Struct OPS in ObjectProperties)
			{
				if(OPS.GO==null)
				{
					continue;
				}

				Transform T=OPS.GO.transform;

				if(T==null)
				{
					continue;
				}

				if(bKeepChildrenPosition)
				{
					T.position=OPS.Position;
				}

				if(bKeepChildrenRotation)
				{
					T.rotation=OPS.Rotation;
				}

				if(bKeepChildrenScale)
				{
					Vector3 V=new Vector3(0,0,0);
					V.x=T.localScale.x*(OPS.Scale.x/T.lossyScale.x);
					V.y=T.localScale.y*(OPS.Scale.y/T.lossyScale.y);
					V.z=T.localScale.z*(OPS.Scale.z/T.lossyScale.z);
					T.localScale=V;
				}
			}

			if(ObjectProperties!=null)
			{
				ObjectProperties.Clear();
			}
		}
		else
		{
			if(bChangePosition)
			{
				MyTransform.localPosition=NewPosition;
			}

			if(bChangeRotation)
			{
				MyTransform.localRotation=NewRotation;
			}

			if(bChangeScale)
			{
				MyTransform.localScale=NewScale;
			}
		}

		if(Children!=null)
		{
			Children.Clear();
		}

		return true;
	}
}
#endif
