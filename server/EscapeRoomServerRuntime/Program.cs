using System.Text.Json;
using RG.EscapeRoom.Model.Rooms;
using RG.EscapeRoomServer.Server;

var serverFactory = new ServerFactory(new ConsoleLogger(), new CancellationTokenSource(), new DotNetRoomDefinitionParser());

var server = serverFactory.CreateServer(12345,"c:/Users/Niklas Gawell/Projects/escape-room/escape-room-vr-client/Assets/RG/EscapeRoomPlayModeTests/SingleLever/SingleLeverFactoryTestRoomDefinition.json", 1000);
server.Start();
while (!server.IsRunning())
{
    Thread.Yield();
}
while (server.IsRunning())
{
    Thread.Yield();
}
public class ConsoleLogger: ILogger {
        public void Info(string message) {
            Console.WriteLine(message);
        }
        public void Error(string error, Exception exception) {
            Console.WriteLine(error);
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
        }
}

public class DotNetRoomDefinitionParser : IRoomDefinitionParser
{
    JsonSerializerOptions options = new JsonSerializerOptions { IncludeFields = true };
    public RoomDefinition Parse(string json)
    {
        var roomDefinition = System.Text.Json.JsonSerializer.Deserialize<RoomDefinition>(json,options);
        Console.WriteLine($"Read room definition {ToJson(roomDefinition)}");
        return roomDefinition;
    }

    public string ToJson(RoomDefinition roomDefinition)
    {
        return System.Text.Json.JsonSerializer.Serialize(roomDefinition, options);
    }
}