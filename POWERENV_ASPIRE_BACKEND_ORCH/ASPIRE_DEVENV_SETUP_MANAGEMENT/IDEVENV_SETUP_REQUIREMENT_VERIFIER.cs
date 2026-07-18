namespace ASPIRE_DEVENV_SETUP_MANAGEMENT
{
    internal interface IDEVENV_SETUP_REQUIREMENT_VERIFIER<TCompliance, TSetup>
    {
        bool CheckCompliance(TCompliance complianceFilter);
        bool SetupRequirement(TSetup setupParameters);
    }
}