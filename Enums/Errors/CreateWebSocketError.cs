namespace ZapMe.Enums.Errors;

public enum CreateWebSocketError
{
    InvalidClientMessage,
    InvalidClientJwt,
    ClientEmailUnverified,
    InvalidClientSession
}