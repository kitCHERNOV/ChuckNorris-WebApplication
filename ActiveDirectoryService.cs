using System;
using System.DirectoryServices.AccountManagement;

namespace BlazorApp1.Services
{
    public class ActiveDirectoryService
    {
        private readonly string _domainName;
        private readonly string _container;

        public ActiveDirectoryService(string domainName, string container)
        {
            _domainName = domainName;
            _container = container;
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, _domainName, _container))
                {
                    return context.ValidateCredentials(username, password);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error authenticating user: " + ex.Message);
                return false;
            }
        }

        public bool IsUserInGroup(string domainName, string username, string password)
        {
            try
            {
                using (var pc = new PrincipalContext(ContextType.Domain, _domainName, username, password))
                {
                    var auth = pc.ValidateCredentials(username, password);

                    if (!auth)
                    {
                        return false;
                    }

                    UserPrincipal user = UserPrincipal.FindByIdentity(pc, username);

                    using (GroupPrincipal gr = GroupPrincipal.FindByIdentity(pc, "MAI-Group"))
                    {
                        var isInGroup = user.IsMemberOf(gr);

                        return isInGroup;
                    }


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking group membership: " + ex.Message);
            }
            return false;
        }
    }
}
