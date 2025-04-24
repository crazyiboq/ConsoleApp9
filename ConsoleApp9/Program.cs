using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
var server = new Server();
var ip = IPAddress.Parse("127.0.0.1");
var port = 27001;

var listener = new TcpListener(ip, port);
listener.Start();
while (true)
{
    var client = listener.AcceptTcpClient();
    var stream = client.GetStream();
    var bw = new BinaryWriter(stream);
    var br = new BinaryReader(stream);

    while (true)
    {
        var input = br.ReadString();
        Console.WriteLine(input);
        var command = JsonSerializer.Deserialize<Command>(input);
        switch (command!.Text)
        {
            case Command.Get:
                var cars = server.GetMethod();
                var json = JsonSerializer.Serialize(cars);
                bw.Write(json);
                break;
            case Command.Post:
                if (command.Car_value != null)
                {
                    server.PostMethod(command.Car_value);
                    bw.Write("✅ Car added successfully.");
                }
                break;

            case Command.Put:
                if (command.Car_value != null)
                {
                    server.PutMethod(command.Car_value);
                    bw.Write("🔄 Car updated.");
                }
                break;
            case Command.Delete:
                if (int.TryParse(command.Param, out int idToDelete))
                {
                    server.DeleteMethod(idToDelete);
                    bw.Write($"🗑️ Car with Id {idToDelete} deleted.");
                }
                break;

            default:
                bw.Write("❌ Unknown command.");
                break;
        }
    }
}
public class Car
{
    public int Id { get; set; }
    public string Name { get; set; }
}

class Command
{
    public const string Post = "POST";
    public const string Get = "GET";
    public const string Delete = "DELETE";
    public const string Put = "PUT";
    public Car Car_value { get; set; }
    public string? Text { get; set; }
    public string? Param { get; set; }

}

public class Server
{
    private List<Car> cars = new List<Car>();

    public List<Car> GetMethod()
    {
        Console.WriteLine($"🔍 Returning {cars.Count} car(s)");
        return cars;
    }
    public void PostMethod(Car car)
    {
        cars.Add(car);
    }
    public void PutMethod(Car car)
    {
        var index = cars.FindIndex(c => c.Id == car.Id);
        if (index != -1)
        {
            cars[index] = car;
        }
    }
    public void DeleteMethod(int id)
    {
        var car = cars.FirstOrDefault(c => c.Id == id);
        if (car != null)
        {
            cars.Remove(car);
        }
    }

}