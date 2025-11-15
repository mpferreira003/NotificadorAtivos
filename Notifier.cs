using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum State{
    None,               // estado inicial
    Abaixo,             // estado em que o ativo está abaixo do limite inferior
    Meio,               // estado do meio
    Acima               // estado em que o ativo está acima do limite superior
}

public class Verification{
    public string ativo;
    public double down_limit;
    public double up_limit;
    
    public Verification(string ativo, double down_limit, double up_limit){
        this.ativo = ativo;
        this.down_limit = down_limit;
        this.up_limit = up_limit;
    }
    
    public State getState(double price){
        if(price>up_limit){
            return State.Acima;
        }else if(price<down_limit){
            return State.Abaixo;
        }else{
            return State.Meio;
        }
    }
    
    public DateTime last_notify_time;
    public State last_state = State.None;
}

public class Notifier{
    
    Provider provider;
    EmailSender sender;
    
    Config config;
    
    List<Verification> verifications = new List<Verification>();
    
    
    public Notifier(Config config,
                    Provider provider,
                    EmailSender sender){
        this.config=config;
        this.provider = provider;
        this.sender = sender;
    }
    
    public void addVerification(Verification v){
        verifications.Add(v);
    }
    
    // retorna se precisa notificar
    public bool MustNotify(Verification v,State state_now){
        if(v.last_state==State.None){
            // estado inicial, então só compara sem depender do tempo
            if(state_now==State.Abaixo || state_now==State.Acima){
                return true;
            }
            return false; // se não estiver acima nem abaixo, não precisa notificar
        }else{
            // caso a verificação já esteja rodando há algum tempo (last_state!=None)
            
            // casos em que precisa considerar o tempo:
            // caso 1: estava abaixo e continua abaixo
            // caso 2: estava acima e continua acima
            if((state_now==State.Abaixo && v.last_state==State.Abaixo) || 
               (state_now==State.Acima  && v.last_state==State.Acima)){
                // caso se tenham passado o tempo determinado na config, notifica
                if(config.ResendEmailTime < (DateTime.UtcNow - v.last_notify_time).TotalSeconds){
                    return true;
                }
                return false;
            }
            
            // Demais casos: 
            // passou de algum estado pra outro (meio->cima, meio->abaixo)
            if(state_now!=State.Meio){ 
                // não é necessário notificar caso tenha mudado para o estado do meio
                // o estado final não é do meio e já foram considerados os casos em que ele 
                // se manteu, então vai ter que notificar de qualquer jeito
                return true;
            }
            return false;
        }
    }
    
    public async Task loop(bool verbose=false){
        while (true){
            await Task.Delay(1000*config.VerificationDelay);
            foreach(Verification v in verifications){
                try{
                    double? price = await provider.GetPrice(v.ativo);
                    // caso algo tenha dado de errado com o provider
                    if (price is null){
                        if(verbose){Console.Write($"Preço do ativo {v.ativo} é null");}
                        continue;
                    }
                    if(verbose){Console.WriteLine($"[{DateTime.UtcNow:T}] [{v.ativo}] Current price = {price}");}
                
                    State state_now = v.getState(price.Value);
                    bool must_notify = MustNotify(v,state_now);
                    v.last_state = state_now; // de qualquer forma, atualiza o estado
                    
                    if(must_notify){
                        // pega a mensagem dependendo do estado que tem que notificar
                        Message template = (state_now==State.Abaixo)?config.downMessage : config.upMessage;
                        Message message = new Message { subject = template.subject, body = "<html>"+template.body };
                        
                        // coloca o texto no final dizendo qual é o ativo e a hora que foi detectado isso
                        message.body += "<br>";
                        message.body += $"<br>Nome do ativo: {v.ativo}.";
                        message.body += $"<br>Datetime: {DateTime.UtcNow:T}.";
                        message.body += $"<br>Preço atual: {price:G}.";
                        message.body += "<html>";
                        
                        if(verbose){Console.WriteLine($"Notificação enviada sobre {v.ativo}. Current price = {price}");}
                        
                        sender.setMessage(message);
                        await sender.sendAsync();
                        
                        // Altera a marcação de quando foi a última atualização que disparou email
                        v.last_notify_time = DateTime.UtcNow;
                    }
                    
                }catch (Exception ex){
                    Console.WriteLine($"ERROR: {ex.Message}");
                }
            }
        }
    }
}
