namespace DoubleMF.Data.Services
{
    public interface ISayHello
    {
        string Hello(string name);
    }
    public interface IRepository
    {
        string GetData();
    }
}