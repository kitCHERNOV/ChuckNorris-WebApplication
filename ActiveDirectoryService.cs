using Microsoft.Extensions.Configuration;

public class ActiveDirectoryService
{
    private readonly string _domain;
    private readonly string _container;
    private readonly string _group;

    public ActiveDirectoryService(IConfiguration configuration)
    {
        var adConfig = configuration.GetSection("ActiveDirectory");
        _domain = adConfig["Domain"];
        _container = adConfig["Container"];
        _group = adConfig["Group"];
    }

    public bool ValidateUser(string username, string password)
    {
        // Only validate if the username and password are both "erica"
        return username == "erica" && password == "erica";
    }

    public bool IsUserInGroup(string username)
    {
        // Only check group membership for "erica"
        return username == "erica";
    }
}
