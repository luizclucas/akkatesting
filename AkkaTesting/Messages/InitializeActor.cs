namespace AkkaTesting.Messages
{
    public class InitializeActor
    {
        public InitializeActor(int workerCounter)
        {
            WorkerCounter = workerCounter;
        }

        public int WorkerCounter { get; set; } = 5;     
    }
}
