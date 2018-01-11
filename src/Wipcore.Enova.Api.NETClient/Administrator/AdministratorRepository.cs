using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Administrator;

namespace Wipcore.Enova.Api.NetClient.Administrator
{
    public class AdministratorRepository<TAdministratorModel> where TAdministratorModel : AdministratorModel 
    {

        private readonly IApiRepository _apiRepository;
        private readonly Func<IApiClient> _apiClient;

        public AdministratorRepository(IApiRepository apiRepository, Func<IApiClient> apiClient)
        {
            _apiRepository = apiRepository;
            _apiClient = apiClient;
        }

        public bool IsLoggedIn()
        {
            var loggedInInfo = _apiClient.Invoke().GetLoggedInUserInfo();
            return Convert.ToBoolean(loggedInInfo["LoggedIn"]);
        }

        public bool IsLoggedInAsAdmin()
        {
            var loggedInInfo = _apiClient.Invoke().GetLoggedInUserInfo();
            return Convert.ToBoolean(loggedInInfo["LoggedIn"]) && loggedInInfo["Role"] == "admin";
        }

        public ILoginResponseModel LoginAdmin(string alias, string password) => _apiClient.Invoke().LoginAdmin(alias, password);

        public IDictionary<string, string> GetLoggedInUserInfo() => _apiClient.Invoke().GetLoggedInUserInfo();
    }
}
