using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using pGina.Shared.Types;

using log4net;

namespace pGina.Plugin.HelloPlugin
{
    public class PluginImpl : pGina.Shared.Interfaces.IPluginAuthentication
    {
        private ILog m_logger;

        private static readonly Guid m_uuid = new Guid("CED8D126-9121-4CD2-86DE-3D84E4A2625E");

        public PluginImpl()
        {
            m_logger = LogManager.GetLogger("pGina.Plugin.HelloPlugin");
        }

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
            get { return m_uuid; }
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
                m_logger.InfoFormat("Successfully authenticated {0}", userInfo.Username);
                return new BooleanResult() { Success = true };
            }
            // Authentication failure
            m_logger.ErrorFormat("Authentication failed for {0}", userInfo.Username);
            return new BooleanResult() { Success = false, Message = "Incorrect username or password." };
        }
    }
}
