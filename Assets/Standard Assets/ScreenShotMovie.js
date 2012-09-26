// The folder we place all screenshots inside.
// If the folder exists we will append numbers to create an empty folder.
var folder = "ScreenshotFolder";
var frameRate = 25;

private var realFolder = "";

function Start () {
    // Set the playback framerate!
    // (real time doesn't influence time anymore)
    Time.captureFramerate = frameRate;

    // Find a folder that doesn't exist yet by appending numbers!
    realFolder = folder;
    var count = 1;
    while (System.IO.Directory.Exists(realFolder)) {
        realFolder = folder + count;
        count++;
    }
    // Create the folder
    System.IO.Directory.CreateDirectory(realFolder);
}

function Update () {
    // name is "realFolder/0005 shot.png"
    var name = String.Format("{0}/{1:D04} shot.png", realFolder, Time.frameCount );

    // Capture the screenshot
    Application.CaptureScreenshot (name);
}
