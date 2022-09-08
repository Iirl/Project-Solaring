using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using solar_a;

/// <summary>
/// Component extentions.
/// ・Copy
/// ・Move (Copy and Delete)
/// </summary>
public static class ComponentExtentions
{
	/// <summary>
	/// Copy Component
	/// 参照URL:
	/// http://answers.unity3d.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
	/// </summary>
	/// <returns>The copy of.</returns>
	/// <param name="comp">Comp.</param>
	/// <param name="other">Other.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T CopyComponent<T> (this Component component, T orgComponent) where T : Component
	{
		Type type = component.GetType ();
		if (type != orgComponent.GetType ()) {
			// type mis-match
			return null;
		}

		// Target
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

		// Propertyを取得
		PropertyInfo[] propInfoList = type.GetProperties (flags);
		foreach (var propInfo in propInfoList) {
			// プロパティに書き込むことができる場合は true。それ以外の場合は false。
			// プロパティに set アクセサーがない場合は、書き込むことができません。
			if (propInfo.CanWrite) {
				try {
					// SetValueの第三引数、GetValueの第二引数は、プロパティの引数。ない場合はnull
					propInfo.SetValue (component, propInfo.GetValue (orgComponent, null), null);
				} catch {
					// In case of NotImplementedException being thrown. 
					//For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
				}   
			}
		}

		// フィールドの属性を取得し、フィールドのメタデータにアクセスできるようにする
		FieldInfo[] finfos = type.GetFields (flags);
		foreach (var finfo in finfos) {
			finfo.SetValue (component, finfo.GetValue (orgComponent));
		}
		return component as T;
	}

	/// <summary>
	/// copyをAddComponent
	/// </summary>
	/// <returns>The component.</returns>
	/// <param name="go">追加対象GameObject</param>
	/// <param name="toAddComponent">AddするCopy元Component</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T AddComponent<T> (this GameObject go, T toAddComponent) where T : Component
	{
		// オリジナルを元にAddする
		return go.AddComponent<T> ().CopyComponent (toAddComponent) as T;
	}

	/// <summary>
	/// copyをMove
	/// → AddComponentして削除する
	/// </summary>
	/// <returns>The component.</returns>
	/// <param name="go">移動先対象GameObject</param>
	/// <param name="toMoveComponent">MoveするCopy元Component</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T MoveComponent<T> (this GameObject go, T toMoveComponent) where T : Component
	{
		// オリジナルを元にコピーする
		T copy = go.AddComponent<T> (toMoveComponent);

		// 移動後は削除する
		UnityEngine.Component.Destroy (toMoveComponent);

		return copy as T;
	}
}