using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Area))]
[CanEditMultipleObjects]
public class AreaEditor : Editor
{
	private SerializedObject TargetSO;
	private SerializedProperty PerimeterPoints;
	private ReorderableList _RL;
	private bool PerimeterPointsUnFolded = false;

	private void OnEnable()
	{
		TargetSO = new SerializedObject(target);
		PerimeterPoints = TargetSO.FindProperty("PerimeterPoints");
		_RL = new ReorderableList(serializedObject, PerimeterPoints, true, true, false, true);
		_RL.drawHeaderCallback = DrawHeaderCallback;
		_RL.drawElementCallback = DrawElementCallback;
		_RL.elementHeightCallback += ElementHeightCallback;
		_RL.onChangedCallback += OnChangeCallback;
		_RL.onCanRemoveCallback = onCanRemoveCallback;
	}
	public override void OnInspectorGUI()
	{
		TargetSO.Update();
		PerimeterPointsUnFolded = EditorGUILayout.Foldout(PerimeterPointsUnFolded, new GUIContent("Perimeter Points"), true);
		if (PerimeterPointsUnFolded) { _RL.DoLayoutList(); }
		if (GUILayout.Button("Apply All Modifiers"))
		{
			(target as Area).ApplyAllModifiers();
		}
		TargetSO.ApplyModifiedProperties();
	}

	private void DrawHeaderCallback(Rect rect)
	{
		EditorGUI.LabelField(rect, "Perimeter Points");
	}
	private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
	{
		EditorGUI.BeginChangeCheck();
		EditorGUI.PropertyField(rect, PerimeterPoints.GetArrayElementAtIndex(index), new GUIContent($"{index}:"));
		if (EditorGUI.EndChangeCheck())
		{
			(target as Area).GetComponents<IAreaAutoUpdate>().ToList().ForEach(A => A.AreaUpdate());
		}
	}
	private float ElementHeightCallback(int index)
	{
		//Gets the height of the element. This also accounts for properties that can be expanded, like structs.
		float propertyHeight =
			EditorGUI.GetPropertyHeight(_RL.serializedProperty.GetArrayElementAtIndex(index), true);

		float spacing = EditorGUIUtility.singleLineHeight / 2;

		return propertyHeight + spacing;
	}
	private void OnChangeCallback(ReorderableList list) => Undo.RecordObject(target, "Changed Peremeter Points");
	private bool onCanRemoveCallback(ReorderableList list) => list.serializedProperty.arraySize > 3;

	private void OnSceneGUI()
	{
		for (int i = 0; i < (target as Area).PerimeterPoints.Count; i++)
		{
			Handles.color = Color.green;
			if (Handles.Button((target as Area).transform.TransformPoint((target as Area).PerimeterPoints[i]),
				Quaternion.Euler((target as Area).transform.forward), 0.2f, 0.2f, Handles.SphereHandleCap))
			{ _RL.index = i; }
			Handles.color = Color.white;
			if (i == _RL.index)
			{
				EditorGUI.BeginChangeCheck();
				//Position Handle
				var newPos = Handles.PositionHandle(
						(target as Area).transform.TransformPoint((target as Area).PerimeterPoints[i])
						, Quaternion.identity);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target, "Changed Peremeter Points");
					newPos = (target as Area).transform.InverseTransformPoint(newPos);
					(target as Area).PerimeterPoints[i] = newPos;
					(target as Area).GetComponents<IAreaAutoUpdate>().ToList().ForEach(A => A.AreaUpdate());
				}
				if (Handles.Button((target as Area).transform.TransformPoint(
				Vector3.Lerp((target as Area).PerimeterPoints[i],
					(target as Area).PerimeterPoints[(i + 1 < (target as Area).PerimeterPoints.Count) ? i + 1 : 0], 0.5f)),
				Quaternion.Euler((target as Area).transform.forward), 0.2f, 0.2f, Handles.SphereHandleCap))
				{
					Undo.RecordObject(target, "Changed Peremeter Points");
					(target as Area).PerimeterPoints.Insert(i + 1, Vector3.Lerp((target as Area).PerimeterPoints[i],
					(target as Area).PerimeterPoints[(i + 1 < (target as Area).PerimeterPoints.Count) ? i + 1 : 0], 0.5f));
					_RL.index = i + 1;
					(target as Area).GetComponents<IAreaAutoUpdate>().ToList().ForEach(A => A.AreaUpdate());
				}
			}
			//Labels
			Handles.Label((target as Area).transform.TransformPoint((target as Area).PerimeterPoints[i]), i.ToString());
		}
		//delete Points with backspace
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Backspace && _RL.index > -1)
		{
			Undo.RecordObject(target, "Changed Peremeter Points");
			(target as Area).PerimeterPoints.RemoveAt(_RL.index);
		}
		Handles.color = Color.red;
		Handles.DrawAAPolyLine((target as Area).PerimeterPoints.ConvertAll(P => (target as Area).transform.TransformPoint(P)).ToArray());
		Handles.DrawDottedLine(
			(target as Area).transform.TransformPoint((target as Area).PerimeterPoints[0]),
			(target as Area).transform.TransformPoint((target as Area).PerimeterPoints[(target as Area).PerimeterPoints.Count - 1]), 10);
		TargetSO.ApplyModifiedProperties();
	}
}
