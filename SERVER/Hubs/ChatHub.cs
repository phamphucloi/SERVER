using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Data.SqlClient;

namespace SERVER.Hubs;

public class ChatHub : Hub
{

    public async Task JoinChat(UserConnection userConnection)
    {
        await SaveConnectionID(userConnection);
        await Clients.All.SendAsync("ReceiveMessage", userConnection.UserName, userConnection.UserName + " has joinchat with ID " + userConnection.ConnectionID);
    }

    public async Task SendMessage(string txtReceiver, string txtSender, string Message)
    {
        string connectionID = getConnectionByReceiver(txtReceiver);
        await Clients.Client(connectionID).SendAsync("ReceiveMessage", txtSender, Message);
    }
    string conStr = "Server=172.16.10.18,14332;Database=DB_INTERN_TRAINING;User Id=loi.pham;Password=Dpt@3003";
    private async Task SaveConnectionID(UserConnection userConnection)
    {
        try
        {
            SqlConnection con = new SqlConnection(conStr);
            await con.OpenAsync();
            SqlCommand sqlCommand = new SqlCommand("deskSaveConnectionUser", con);
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@UserName", userConnection.UserName);
            sqlCommand.Parameters.AddWithValue("@ConnectionId", userConnection.ConnectionID);
            await sqlCommand.ExecuteNonQueryAsync();
        
        }
        catch (Exception ex)
        {

        }
    }

    private string getConnectionByReceiver(string txtReceiver)
    {
        SqlDataAdapter adapter = new SqlDataAdapter($"SELECT CONNECTION_ID FROM userConnection WHERE USER_NAME = {txtReceiver}", conStr);
        using (DataTable dt = new DataTable())
        {
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }

            return "";
        }
    
    }

}

public class UserConnection
{
    public string UserName { get; set; }
    public string ConnectionID { get; set; }
}
