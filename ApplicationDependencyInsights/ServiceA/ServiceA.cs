namespace ServiceA
{
    public class ServiceA
    {
        public void Run()
        {
            var b = new ServiceB();
            b.Execute();
        }
    }
