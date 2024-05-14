using Neo4j.Driver;


namespace WebApplication1.Properties;

public class MyNeo4j
{
    private static IDriver _driver = null;
    public static IDriver Driver => _driver;

    public static async Task InitDriverAsync(string uri, string username, string password)
    {
        _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
        await _driver.VerifyConnectivityAsync();
        //Console.WriteLine("S a realizat conexiunea la baza de date");
    }
    public static Task CloseDriver()
    {
        return _driver != null ? _driver.CloseAsync() : Task.CompletedTask;
    }
}