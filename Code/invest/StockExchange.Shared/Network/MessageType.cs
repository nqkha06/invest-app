namespace StockExchange.Shared.Network;

public enum MessageType
{
    Login = 1,
    Register = 2,
    GetProfile = 3,
    UpdateProfile = 4,
    AdminGetUsers = 20,
    AdminCreateUser = 21,
    AdminUpdateUser = 22
}
