public sealed class UsernameAlreadyExistsException : Exception
{
    public UsernameAlreadyExistsException()
        : base("Username already exists.")
    {

    }
}