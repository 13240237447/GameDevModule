using System.Collections.Generic;
using Logic;
using UnityEngine;


namespace LogicTest
{
    public class ActivityTest : MonoBehaviour
    {
        private Entity activityController;

        private bool isActivityStop;
        
        private void Awake()
        {
            TestScene scene = new TestScene();
            DataEntity dataEntity = new DataEntity();
            RenderEntity renderEntity = new RenderEntity(dataEntity);
            Game.World.AddScene(scene);
            activityController = scene.CreateEntity(new List<Activity>()
            {
                new CallFunc(() => { Debug.Log("Activity Start"); },false),
                new Wait(3,false),
                new CallFunc(() => { Debug.Log("Pass 3 Second"); },false),
                new Wait(2,false),
                new WaitFor(()=>isActivityStop),
                new CallFunc(() => { Debug.Log("Activity End"); },false),
            });

            Debug.Log(activityController.CurrentActivity.PrintActivityTree(activityController));
       
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
            
            Game.World.TickOuter();
        }
        
    }
}
