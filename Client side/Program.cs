using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var ip = IPAddress.Parse("127.0.0.1");
var port = 27001;

var client = new TcpClient();
client.Connect(ip, port);

var stream = client.GetStream();
var bw = new BinaryWriter(stream);
var br = new BinaryReader(stream);



while (true)
{
    Console.WriteLine("Enter command (POST/PUT/GET/DELETE):");
    var str = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(str)) continue;

    var input = str.Split(' ');
    var command = new Command();

    switch (input[0].ToUpper())
    {
        case Command.Post:
            Console.Write("Enter Car Id: ");
            int postId = int.Parse(Console.ReadLine()!);
            Console.Write("Enter Car Name: ");
            string postName = Console.ReadLine()!;
            command.Text = Command.Post;
            command.Car_value = new Car { Id = postId, Name = postName };
            break;

        case Command.Put:
            Console.Write("Enter Car Id to update: ");
            int putId = int.Parse(Console.ReadLine()!);
            Console.Write("Enter New Car Name: ");
            string putName = Console.ReadLine()!;
            command.Text = Command.Put;
            command.Car_value = new Car { Id = putId, Name = putName };
            break;
        case Command.Get:
            command.Text = Command.Get;
            var jsonGet = JsonSerializer.Serialize(command);
            bw.Write(jsonGet);

            var responc = br.ReadString();
            var cars = JsonSerializer.Deserialize<List<Car>>(responc);

            Console.WriteLine("🚗 Cars in server:");
            foreach (var car in cars!)
                Console.WriteLine($"[{car.Id}] {car.Name}");
            break;

        case Command.Delete:
            Console.Write("Enter Car Id to delete: ");
            string idToDelete = Console.ReadLine()!;
            command.Text = Command.Delete;
            command.Param = idToDelete;
            break;
        default:
            Console.WriteLine("❌ Invalid command.");
            continue;
    }
    var json = JsonSerializer.Serialize(command);
    bw.Write(json);
    var response = br.ReadString();
    if (string.IsNullOrWhiteSpace(response))
    {
        Console.WriteLine("❌ No response from the server.");
        continue;
    }
    Console.WriteLine("Press any key to continue");
    Console.ReadKey();
    Console.Clear();
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


