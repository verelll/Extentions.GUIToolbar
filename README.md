Gui extention script that allows you to add GUI elements to the editor toolbar.


Example:

![image](https://github.com/verelll/Extentions.GUIToolbar/assets/77948801/428878f0-a544-4b6a-a777-0d0d8abdc966)

    [InitializeOnLoad]
    public static class ExampleToolbar
    {
        private static bool _testToggle;
        
        static ExampleToolbar()
        {
            GUIToolbarExtender.AddLeftGUIHandler(OnLeftToolbarGUI);
            GUIToolbarExtender.AddRightGUIHandler(OnRightToolbarGUI);
        }

        private static void OnLeftToolbarGUI()
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Test Button")) { }

            _testToggle = GUILayout.Toggle(_testToggle, "Test Toggle");
        }
		
        private static void OnRightToolbarGUI()
        {
            if (GUILayout.Button("Test Play Right")) { }
			
            if (GUILayout.Button("Test Play")) { }
        }
    }




