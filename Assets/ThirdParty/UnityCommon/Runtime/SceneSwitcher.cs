// Copyright 2012-2018 Elringus (Artyom Sovetnikov). All Rights Reserved.

namespace UnityCommon
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    
    public class SceneSwitcher : MonoBehaviour 
    {
        private const int BUTTON_HEIGHT = 50;
        private const int BUTTON_WIDTH = 150;
    
        private void OnGUI ()
    	{
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var xPos = Screen.width - BUTTON_WIDTH;
                var yPos = Screen.height - (BUTTON_HEIGHT + (BUTTON_HEIGHT * i));
                var scene = SceneUtility.GetScenePathByBuildIndex(i).GetAfter("/").GetBefore(".");
                if (GUI.Button(new Rect(xPos, yPos, BUTTON_WIDTH, BUTTON_HEIGHT), scene)) SceneManager.LoadScene(i);
            }
    	}
    }
    
}
