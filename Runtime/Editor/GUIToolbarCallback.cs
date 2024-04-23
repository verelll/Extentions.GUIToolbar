using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.UIElements;

namespace Extentions.GUIToolbar
{
	internal static class GUIToolbarCallback
	{
		private const string ToolbarTypeName = "UnityEditor.Toolbar";
		private const string RootToolbarFieldName = "m_Root";
		private const string ToolbarZoneLeftAlignName = "ToolbarZoneLeftAlign";
		private const string ToolbarZoneRightAlignName = "ToolbarZoneRightAlign";
		
		private static readonly Type _toolbarType = typeof(Editor).Assembly.GetType(ToolbarTypeName);

		private static ScriptableObject m_currentToolbar;

		internal static Action OnToolbarGUI;
		internal static Action OnToolbarGUILeft;
		internal static Action OnToolbarGUIRight;
		
		static GUIToolbarCallback()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
		}

		private static void OnUpdate()
		{
			if (m_currentToolbar != null) 
				return;
			
			var toolbars = Resources.FindObjectsOfTypeAll(_toolbarType);
			m_currentToolbar = toolbars.Length > 0 
				? (ScriptableObject) toolbars[0] 
				: null;
			
			if (m_currentToolbar == null) 
				return;
			
			var root = m_currentToolbar.GetType()
				.GetField(RootToolbarFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
			
			var rawRoot = root.GetValue(m_currentToolbar);
			var mRoot = rawRoot as VisualElement;
			
			RegisterCallback(ToolbarZoneLeftAlignName, OnToolbarGUILeft);
			RegisterCallback(ToolbarZoneRightAlignName, OnToolbarGUIRight);

			void RegisterCallback(string root, Action cb) 
			{
				var toolbarZone = mRoot.Q(root);
				var parent = new VisualElement()
				{
					style = {
						flexGrow = 1,
						flexDirection = FlexDirection.Row,
					}
				};
				var container = new IMGUIContainer();
				container.style.flexGrow = 1;
				container.onGUIHandler += () => { 
					cb?.Invoke();
				}; 
				parent.Add(container);
				toolbarZone.Add(parent);
			}
		}
	}
}
