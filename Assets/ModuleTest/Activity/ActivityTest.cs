using System;
using System.Collections.Generic;
using Module.Activity;
using UnityEngine;


namespace ModuleTest.Activity
{
    public class ActivityTest : MonoBehaviour
    {
        private ActivityContainer activityContainer;

        private bool isActivityStop;
        private void Awake()
        {
            activityContainer = new ActivityContainer(new List<Module.Activity.Activity>()
            {
                new CallFunc(() => { Debug.Log("Activity Start"); },false),
                new Wait(3,false),
                new CallFunc(() => { Debug.Log("Pass 3 Second"); },false),
                new Wait(2,false),
                new WaitFor(()=>isActivityStop),
                new CallFunc(() => { Debug.Log("Activity End"); },false),
            });

            Debug.Log( activityContainer.CurrentActivity.PrintActivityTree(activityContainer));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                isActivityStop = true;
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                activityContainer.CancelActivity();
            }
            activityContainer.Tick();
        }
        
    }
}
