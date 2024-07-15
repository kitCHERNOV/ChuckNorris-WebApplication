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

        public bool IsUserInGroup(string username) 
        {
            try 
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, _domainName, _container)) // Создание контекста для подключения к домену с указанным именем и контейнером
                {
                    using (UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username)) // Поиск пользователя в домене по его имени
                    {
                        if (user != null) 
                        {
                            using (GroupPrincipal group = GroupPrincipal.FindByIdentity(context, "MAI-Group")) // Поиск группы "MAI-Group" в домене
                            {
                                if (group != null) 
                                {
                                    return user.IsMemberOf(group); // Возврат результата проверки членства пользователя в группе
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) // Начало блока catch для обработки исключений
            {
                Console.WriteLine("Error checking group membership: " + ex.Message); // Вывод сообщения об ошибке в консоль
            }
            return false; // Возврат false, если произошла ошибка или пользователь/группа не найдены
        }
    }
}
