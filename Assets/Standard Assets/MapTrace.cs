using UnityEngine;
using System.Collections;
using System.IO;

public class MapTrace : MonoBehaviour {

    public GameObject roomNodeAsset;
    public GameObject roomNodeLineAsset;

    string[] roomPaths;
    string letters = "ABCDEFGHIJKLMNOP";
    ArrayList doors = new ArrayList();

    void Awake () {
        DontDestroyOnLoad(gameObject);
    }

    void Start () {
        StartCoroutine(DrawMap());
    }

    Vector3 GetRoomPos (string room) {
        int x = 3 * letters.IndexOf(room[0]);
        int y = -3 * letters.IndexOf(room[1]);
        int z = 0;
        if (room.Length == 4) {
            Vector3 a = GetRoomPos(room.Substring(0, 2));
            Vector3 b = GetRoomPos(room.Substring(2, 2));
            return (a + b) / 2;
        }
        else if (room.Length != 2) {
            z = 3;
        }
        return new Vector3(x, y, z);
    }

    IEnumerator DrawMap () {
        yield return new WaitForSeconds(1.0f);
        TextReader mapTrace = new StreamReader(Application.dataPath + "/Tests/MapTrace.txt");
        string line;
        string room = "";
        while ((line = mapTrace.ReadLine()) != null) {
            if (line[0] == '[') {
                room = line.Substring(1, line.Length - 2);
                GameObject roomObject = (GameObject)Instantiate(roomNodeAsset);
                roomObject.name = room;
                roomObject.transform.position = GetRoomPos(room);
                TextMesh textMesh = (TextMesh)roomObject.GetComponent(typeof(TextMesh));
                textMesh.text = room;
            }
            else if (line.StartsWith("Door|")) {
                string[] doorsInfo = line.Split('>');
                doors.Add(line);
                string[] thisDoorInfo = doorsInfo[0].Split('|');
                string[] nextDoorInfo = doorsInfo[1].Split('|');
                if (nextDoorInfo.Length == 3) {
                    GameObject lineObject = (GameObject)Instantiate(roomNodeLineAsset);
                    lineObject.name = line;
                    LineRenderer lineRenderer = (LineRenderer)lineObject.GetComponent(typeof(LineRenderer));
                    lineRenderer.SetPosition(0, GetRoomPos(thisDoorInfo[1]));
                    lineRenderer.SetPosition(1, GetRoomPos(nextDoorInfo[1]));
                }
            }
            else if (line.StartsWith("EnergyTank|")) {
                string[] energyInfo = line.Split('|');
                if (energyInfo.Length == 2) {
                    GameObject e = (GameObject)Instantiate(Resources.Load("EnergyTank"));
                    Destroy(e.GetComponent(typeof(Collider)));
                    Destroy(e.GetComponent(typeof(EnergyTank)));
                    e.name = line;
                    e.transform.rotation = Quaternion.Euler(270,180,0);
                    e.transform.position = GetRoomPos(energyInfo[1]) + new Vector3(0.5f,-0.5f,0);
                }
            }
        }
    }

   	void Update () {
	
	}
}
