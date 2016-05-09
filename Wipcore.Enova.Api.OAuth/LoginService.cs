using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.OAuth
{
    public class LoginService
    {
        public User Login(LoginModel model)
        {
            try
            {
                var context = EnovaSystemFacade.Current.Connection.CreateContext();

                //context.Login("wadmin", "wadmin");
                //var customer = EnovaCustomer.Find(context, model.Username);
                //customer.Edit();
                //customer.Password = model.Password;
                //customer.Save();
                //context.Logout();

                if (model.IsAdmin)
                {
                    return context.Login(model.Username, model.Password);
                }
                else
                {
                    return context.CustomerLogin(model.Username, model.Password);
                }

            }
            catch (Exception)
            {
                //TODO log
                return null;
            }
            
        }
    }
}
