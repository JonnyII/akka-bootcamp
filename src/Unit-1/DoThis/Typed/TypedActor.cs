namespace WinTail.Typed;


interface ITestActorCommand { }
class TestActor : Actor<TestActor, ITestActorCommand>
{
    protected override void OnReceive(ITestActorCommand message)
    {

    }
}
class DependentTestActor : Actor<TestActor, ITestActorCommand>
{
    protected override void OnReceive(ITestActorCommand message)
    {

    }

    public DependentTestActor(IActorRef<TestActor, ITestActorCommand> testActor)
    {

    }
}

