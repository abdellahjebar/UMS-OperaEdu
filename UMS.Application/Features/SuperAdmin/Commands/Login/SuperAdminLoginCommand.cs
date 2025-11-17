using MediatR;

namespace UMS.Application.Features.SuperAdmin.Commands.Login
{
    /// <summary>
    /// Command to authenticate SuperAdmin and generate JWT token
    /// </summary>
    public record SuperAdminLoginCommand(string Email, string Password) : IRequest<SuperAdminLoginResponse>;

    /// <summary>
    /// Response containing JWT token and SuperAdmin information
    /// </summary>
    public record SuperAdminLoginResponse(
        string Token,
        string Email,
        string FullName,
        string Message
    );
}
