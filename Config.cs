public class Config
{
    public List<string> destiny { get; set; }
    public Message upMessage { get; set; }
    public Message downMessage { get; set; }
    public int ResendEmailTime {get; set; }     // tempo até notificar novamente (em minutos)
    public int VerificationDelay {get; set; }   // tempo entre cada verificação (em segundos)
}

public class Message
{
    public string subject { get; set; }
    public string body { get; set; }
}