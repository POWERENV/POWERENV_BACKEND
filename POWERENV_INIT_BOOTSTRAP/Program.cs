using POWERENV_INIT_BOOTSTRAP.DEVENV_SETUP_MANAGEMENT;

namespace POWERENV_INIT_BOOTSTRAP
{
    public class Program
    {
        static void Main(string[] args)
        {
            bool isPublishingMode;
            if (!bool.TryParse(args[0], out isPublishingMode)) return;

            POWERENV_REQUIREMENTS_CHECKER powerenvRequirementsCheck = new POWERENV_REQUIREMENTS_CHECKER(isPublishingMode);
        }
    }
}