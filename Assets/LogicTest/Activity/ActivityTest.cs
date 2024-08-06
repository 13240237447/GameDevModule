using System;
using System.Collections.Generic;
using Logic.Activity;
using UnityEngine;


namespace ModuleTest.Activity
{
    public class ActivityTest : MonoBehaviour
    {
        private ActivityController activityController;

        private bool isActivityStop;
        private void Awake()
        {
            activityController = new ActivityController(new List<Logic.Activity.Activity>()
            {
                new CallFunc(() => { Debug.Log("Activity Start"); },false),
                new Wait(3,false),
                new CallFunc(() => { Debug.Log("Pass 3 Second"); },false),
                new Wait(2,false),
                new WaitFor(()=>isActivityStop),
                new CallFunc(() => { Debug.Log("Activity End"); },false),
            });

            Debug.Log( activityController.CurrentActivity.PrintActivityTree(activityController));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                isActivityStop = true;
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                activityController.CancelActivity();
            }
            activityController.Tick();
        }
        
    }
}
