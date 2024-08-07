using Logic;
using LogicTest;

public static class Game
{
    public static World World { private set; get; } = new World();

    public static void Test()
    {
        TestScene scene = new TestScene();
        DataEntity dataEntity = new DataEntity();
        RenderEntity renderEntity = new RenderEntity(dataEntity);
        scene.AddEntity(dataEntity);
        scene.AddEntity(renderEntity);
        World.AddScene(scene);
    }
}
