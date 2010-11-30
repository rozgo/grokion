using UnityEngine;
using System.Collections;

public class Sway : MonoBehaviour {
   
   private Vector3 pOrig;
   private Vector3 pNow;
   private Vector3 pLast;
   
   private float inversemass= 0.001f;
   private float LinearStiffness = 1000000.0f;
   private float LinearDamping = 0.35f;
   
   private Quaternion origRot;
   
   void Awake()
   {
      print("Awake...");   
      pOrig = transform.localPosition;
      origRot = transform.localRotation;
      pLast = new Vector3(0,0,0);
   }

   void LateUpdate ()
   {
      Vector3 LinearAcceleration;
      Vector3 tmpV;
      LinearAcceleration =  pNow*(LinearStiffness*inversemass);
      tmpV = pNow;
      pNow = (2.0f - LinearDamping)*pNow-(1.0f-LinearDamping)*pLast - LinearAcceleration * Time.deltaTime * Time.deltaTime;
      pLast = tmpV;
      transform.localPosition = pOrig + pNow;
      transform.localRotation = Quaternion.Lerp(transform.localRotation, origRot, Time.deltaTime * 20);
   }
   void kick()
   {
      print("Kick");
      pLast = transform.localRotation * new Vector3(0.02f,0.0f,0.0f);
      transform.localRotation *= Quaternion.Euler(0, 10, 0);
   }
   
   void Update()
   {
      if(Input.GetKeyDown("k"))
      {
         kick();
      }
   }
} 