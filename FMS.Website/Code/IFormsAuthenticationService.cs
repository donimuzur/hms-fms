namespace FMS.Website.Code
{
    /// <summary>
    /// wraps FormsAuthenticationService to allow unit testing of controllers
    /// </summary>
    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }
}