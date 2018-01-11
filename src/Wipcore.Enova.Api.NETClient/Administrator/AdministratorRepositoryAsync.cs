using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Administrator;

namespace Wipcore.Enova.Api.NetClient.Administrator
{
    public class AdministratorRepositoryAsync<TAdministratorModel> where TAdministratorModel : AdministratorModel
    {

        private readonly IApiRepositoryAsync _apiRepository;
        private readonly Func<IApiClientAsync> _apiClient;

        public AdministratorRepositoryAsync(IApiRepositoryAsync apiRepository, Func<IApiClientAsync> apiClient)
        {
            _apiRepository = apiRepository;
            _apiClient = apiClient;
        }

        public async Task<bool> IsLoggedIn()
        {
            var loggedInInfo = await _apiClient.Invoke().GetLoggedInUserInfoAsync();
            return Convert.ToBoolean(loggedInInfo["LoggedIn"]);
        }
        
        public async Task<bool> IsLoggedInAsAdmin()
        {
            var loggedInInfo = await _apiClient.Invoke().GetLoggedInUserInfoAsync();
            return Convert.ToBoolean(loggedInInfo["LoggedIn"]) && loggedInInfo["Role"] == "admin";
        }

        public async Task<IDictionary<string, string>> GetLoggedInUserInfo()
        {
            return await _apiClient.Invoke().GetLoggedInUserInfoAsync();
        }

        public async Task<ILoginResponseModel> LoginAdmin(string alias, string password) =>
            await _apiClient.Invoke().LoginAdminAsync(alias, password);




    }
}
