namespace SharedKernel.Enums
{
    public enum ErrorCode
    {
        NotFound,
        ValidationError,
        Unauthorized,
        Conflict, // Data conflict (e.g., duplicate entry).
        UnexpectedError
    }
}
