using UnityEngine;

public abstract class AdaptiveWindowGUI : MonoBehaviour
{
    public float LeftMargin = 150f, TopMargin, RightMargin, BottomMargin;

    private Rect windowRect;
    private string className;

    protected abstract void OnWindowGUI (int windowId);

    protected virtual string GetWindowTitle ()
    {
        return className;
    }

    protected virtual int GetWindowId ()
    {
        return 0;
    }

    protected virtual void Awake ()
    {
        className = GetType().Name;
    }

    protected virtual void OnGUI ()
    {
        GUILayout.Window(GetWindowId(), windowRect, OnWindowGUI, GetWindowTitle());
    }

    protected virtual void Update ()
    {
        SetWindowRect();
    }

    protected virtual void SetWindowRect ()
    {
        windowRect = new Rect(LeftMargin, TopMargin, 
            Screen.width - LeftMargin - RightMargin, 
            Screen.height - TopMargin - BottomMargin);
    }
}
