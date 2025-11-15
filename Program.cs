using System;
using System.Text.Json;
using System.Globalization;
using System.Threading.Tasks;
using DotNetEnv;



public partial class Program
{
    static async Task Main(string[] args)
    {           
        const string CONFIG_PATH = "config.json";
        
        if (args.Length != 3)
        {
            Console.WriteLine("Coloque no formato: <ATIVO> <PRICE_UP> <PRICE_DOWN>");
            return;
        }
        
        // Lê argumentos e configurações
        string ativo = args[0];
        float priceUp = float.Parse(args[1]);
        float priceDown = float.Parse(args[2]);
        
        Env.Load();
        string username = Environment.GetEnvironmentVariable("EMAIL");
        string password = Environment.GetEnvironmentVariable("PASSWORD");
        string host = Environment.GetEnvironmentVariable("HOST");
        
        string json = File.ReadAllText(CONFIG_PATH);
        Config config = JsonSerializer.Deserialize<Config>(json);
        
        // cria o notificador
        EmailSender sender = new EmailSender(host,username,password,config.destiny);
        Provider provider = new Provider();
        Notifier notifier = new Notifier(config,provider,sender);
        
        //adiciona as verificações que ele precisa fazer. No caso é só uma, então só dá um  add pra verificar o ativo
        Verification v = new Verification(ativo,priceDown,priceUp);
        notifier.addVerification(v);
        
        // loop de verificação
        await notifier.loop(true);
    }
}
