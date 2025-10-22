namespace FastVocab.Shared.Utils;

public sealed record Error(string Title, int ErrorCode, string? Details = null)
{
    public static Error NotFound = new("Resource not found", 404);
    public static Error Deleted = new("Resource has already been deleted", 400);
    public static Error Conflict = new("Operation conflicts", 400);
    public static Error Duplicate = new("Duplicate", 400);
    public static Error InvalidInput = new("Invalid input", 400);
    public static Error AccessDenied = new("Access denied", 403);
    public static Error ExternalServiceError = new("ExternalError", 503);
    
    // Validation Errors
    public static Error ValidationFailed = new("Validation failed", 400);
    
    /// <summary>
    /// Creates a validation error with specific property and message
    /// </summary>
    public static Error ValidationError(string propertyName, string message) 
        => new("Validation failed", 400, $"{propertyName}: {message}");
    
    /// <summary>
    /// Creates multiple validation errors from property-message pairs
    /// </summary>
    public static List<Error> ValidationErrors(IEnumerable<(string Property, string Message)> failures)
        => failures.Select(f => ValidationError(f.Property, f.Message)).ToList();
}
