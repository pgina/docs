using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using pGina.Shared.Types;

namespace pGina.Plugin.HelloPlugin
{
    public class PluginImpl : pGina.Shared.Interfaces.IPluginAuthentication
    {
        public string Name
        {
            get { return "Hello"; }
        }

        public string Description
        {
            get { return "Authenticates users with 'hello' in the username and 'pGina' in the password"; }
        }

        public Guid Uuid
        {
            get { return new Guid("CED8D126-9121-4CD2-86DE-3D84E4A2625E"); }
        }

        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public void Starting() { }

        public void Stopping() { }

        public BooleanResult AuthenticateUser(SessionProperties properties)
        {
            UserInformation userInfo = properties.GetTrackedSingle<UserInformation>();

            if (userInfo.Username.Contains("hello") && userInfo.Password.Contains("pGina"))
            {
                // Successful authentication
                return new BooleanResult() { Success = true };
            }
            // Authentication failure
            return new BooleanResult() { Success = false, Message = "Incorrect username or password." };
        }
    }
}
