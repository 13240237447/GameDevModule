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
            RegenerationTrait regenerationTrait = new RegenerationTrait();
            HealthTraitRender healthTraitRender = new HealthTraitRender();
            HealthTraitInfo healthTraitInfo = new HealthTraitInfo();
            EntityInfo entityInfo = new EntityInfo(new List<TraitInfo>(){healthTraitInfo});
            Game.World.AddScene(scene);
            activityController = scene.CreateEntity(entityInfo,new List<Activity>()
            {
                new CallFunc(() => { Debug.Log("Activity Start"); },false),
                new Wait(3,false),
                new CallFunc(() => { Debug.Log("Pass 3 Second"); },false),
                new Wait(2,false),
                new WaitFor(()=>isActivityStop),
                new CallFunc(() => { Debug.Log("Activity End"); },false),
            },new List<ITrait>()
            {
                regenerationTrait,
                healthTraitRender,
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
            // activityController.Tick();
            Game.World.TickOuter();
        }
        
    }
}
