using System.Linq;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    private const string FolderName = "Screenshots";
    private string _path;
    private int _number;

    private void Start()
    {
        //TODO: set path
        _path = UnityEngine.Windows.Directory.localFolder;
        
        if (!UnityEngine.Windows.Directory.Exists(FolderName))
        {
            UnityEngine.Windows.Directory.CreateDirectory(FolderName);
        }

        _path += "/" + FolderName;
        while(UnityEngine.Windows.File.Exists(FolderName + "/screen_" + _number + ".png")) ++_number;
        ;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            var name = FolderName + "/screen_" + _number + ".png";
            name = name.Replace('/', '\\');
            ScreenCapture.CaptureScreenshot(name);
            Debug.Log(name);
            _number++;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            var name = FolderName + "/screen_" + _number + ".png";
            name = name.Replace('/', '\\');
            
            Debug.Log(name);
            _number++;
        }
        
    }
}
