using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender{
    public string provider {get; private set; }
    public string username {get; private set; }
    public string password {get; private set; }
    
    // lista de destinatários
    public List<string> dests;
    public void setDests(List<string> dests){this.dests=dests;}
    public List<string> getDests(){return dests;}

    // conteúdo
    Message message;
    public void setMessage(Message message){
        this.message=message;
    }
    public Message getMessage(){
        return message;
    }

    public EmailSender(string provider, 
                    string username, 
                    string password, 
                    List<string> dests){
        this.provider=provider?? throw new ArgumentNullException(nameof(this.provider));
        this.username=username?? throw new ArgumentNullException(nameof(this.username));
        this.password=password?? throw new ArgumentNullException(nameof(this.password));
        setDests(dests);
    }
    
    public MailMessage createMessage(){
        var mail = new MailMessage();
        mail.From = new MailAddress(username);
        foreach (var dest in dests){
            try{
                mail.To.Add(dest);    
            }catch{
                Console.WriteLine("[Warning] - {0:g} cannot be added.",dest);
            }
        }
        mail.Subject = message.subject;
        mail.Body = message.body;
        mail.IsBodyHtml = true;
        return mail;
    }
    
    public async Task sendAsync(){
        using var client = new SmtpClient();
        client.Host = provider;
        client.Port = 587;
        client.EnableSsl = true;
        client.Timeout = 3600000;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(username,password);

        var mail = createMessage();
        await client.SendMailAsync(mail);
    }
}