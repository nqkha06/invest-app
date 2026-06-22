namespace StockExchange.Shared.Network;

public enum MessageType
{
    Login = 1,
    Register = 2,
    GetProfile = 3,
    UpdateProfile = 4,

    GetAllStocks = 5,
    SearchStocks = 6,
    CreateStock = 7,
    UpdateStock = 8,
    DeleteStock = 9,

    AdminGetUsers = 20,
    AdminCreateUser = 21,
    AdminUpdateUser = 22,
    AdminDeleteUser = 23,
    AdminGetDashboard = 24,
    AdminGetSimulations = 25,
    AdminUpdateSimulation = 26
}
