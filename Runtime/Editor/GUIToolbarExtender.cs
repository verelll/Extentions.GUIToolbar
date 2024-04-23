using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Extentions.GUIToolbar
{
	[InitializeOnLoad]
	public static class GUIToolbarExtender
	{
		private const string ToolbarType = "UnityEditor.Toolbar";
		private const string FieldName = "k_ToolCount";
		private const string CommandStyleName = "CommandLeft";
		
		private const float StyleSpace = 8;
		private const float StyleButtonWidth = 32;
		private const float StyleDropdownWidth = 80;
		private const float StylePlayPauseStopWidth = 140;
		
		private static readonly List<Action> _leftToolbarGUIHandlers = new List<Action>();
		private static readonly List<Action> _rightToolbarGUIHandlers = new List<Action>();

		private static readonly int _toolCount;

		private static GUIStyle _commandStyle = null;

		static GUIToolbarExtender()
		{
			var toolbarType = typeof(Editor).Assembly.GetType(ToolbarType);

			var toolIcons = toolbarType.GetField(FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			
			_toolCount = toolIcons != null 
				? (int) toolIcons.GetValue(null) 
				: 8;

			GUIToolbarCallback.OnToolbarGUI = OnGUI;
			GUIToolbarCallback.OnToolbarGUILeft = InvokeLeftHandlers;
			GUIToolbarCallback.OnToolbarGUIRight = InvokeRightHandlers;
		}

		private static void OnGUI()
		{
			_commandStyle ??= new GUIStyle(CommandStyleName);

			var screenWidth = EditorGUIUtility.currentViewWidth;

			// Following calculations match code reflected from Toolbar.OldOnGUI()
			float playButtonsPosition = Mathf.RoundToInt ((screenWidth - StylePlayPauseStopWidth) * 0.5f);

			var leftRect = new Rect(0, 0, screenWidth, Screen.height);
			leftRect.xMin += StyleSpace; // Spacing left
			leftRect.xMin += StyleButtonWidth * _toolCount; // Tool buttons
			
			leftRect.xMin += StyleSpace; // Spacing between tools and pivot

			leftRect.xMin += 64 * 2; // Pivot buttons
			leftRect.xMax = playButtonsPosition;

			var rightRect = new Rect(0, 0, screenWidth, Screen.height);
			rightRect.xMin = playButtonsPosition;
			rightRect.xMin += _commandStyle.fixedWidth * 3; // Play buttons
			rightRect.xMax = screenWidth;
			rightRect.xMax -= StyleSpace; // Spacing right
			rightRect.xMax -= StyleDropdownWidth; // Layout
			rightRect.xMax -= StyleSpace; // Spacing between layout and layers
			rightRect.xMax -= StyleDropdownWidth; // Layers
			
			rightRect.xMax -= StyleSpace; // Spacing between layers and account

			rightRect.xMax -= StyleDropdownWidth; // Account
			rightRect.xMax -= StyleSpace; // Spacing between account and cloud
			rightRect.xMax -= StyleButtonWidth; // Cloud
			rightRect.xMax -= StyleSpace; // Spacing between cloud and collab
			rightRect.xMax -= 78; // Colab

			// Add spacing around existing controls
			leftRect.xMin += StyleSpace;
			leftRect.xMax -= StyleSpace;
			rightRect.xMin += StyleSpace;
			rightRect.xMax -= StyleSpace;

			// Add top and bottom margins
			leftRect.y = 4;
			leftRect.height = 22;
			rightRect.y = 4;
			rightRect.height = 22;

			if (leftRect.width > 0)
			{
				GUILayout.BeginArea(leftRect);
				InvokeLeftHandlers();
				GUILayout.EndArea();
			}

			if (rightRect.width > 0)
			{
				GUILayout.BeginArea(rightRect);
				InvokeRightHandlers();
				GUILayout.EndArea();
			}
		}

		private static void InvokeLeftHandlers()
		{
			GUILayout.BeginHorizontal();
			foreach (var handler in _leftToolbarGUIHandlers)
			{
				handler?.Invoke();
			}
			GUILayout.EndHorizontal();
		}

		private static void InvokeRightHandlers()
		{
			GUILayout.BeginHorizontal();
			foreach (var handler in _rightToolbarGUIHandlers)
			{
				handler?.Invoke();
			}
			GUILayout.EndHorizontal();
		}

		public static void AddLeftGUIHandler(Action handler)
		{
			_leftToolbarGUIHandlers.Add(handler);
		}
		
		public static void RemoveLeftGUIHandler(Action handler)
		{
			_leftToolbarGUIHandlers.Remove(handler);
		}
		
		public static void AddRightGUIHandler(Action handler)
		{
			_rightToolbarGUIHandlers.Add(handler);
		}
		
		public static void RemoveRightGUIHandler(Action handler)
		{
			_rightToolbarGUIHandlers.Remove(handler);
		}
	}
}
