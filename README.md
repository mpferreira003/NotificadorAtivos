# Notificador de Ativos
Este projeto em C# foi desenvolvido com a finalidade de monitorar ativos utilizando através da api
[brapi](https://brapi.dev/), disparando emails caso atinjam threshold específicos.


## Configuração
Este projeto utiliza dois arquivos de configuração fundamentais:
- config.json - responsável pelas configurações funcionais da aplicação (mensagens, destinatários, tempos, etc.).
- .env - armazena informações sensíveis, como host de e-mail, usuário e senha do remetente.

### Alterando config.json
Este arquivo controla o comportamento do sistema de notificação. Segue explicação de cada campo abaixo:
- destiny: lista de emails de destinatários
- upMessage: define a mensagem enviada quando o ativo passa o limite superior. Tem os subcampos "subject" e "body" para definir a mensagem padrão.
- downMessage: define a mensagem enviada quando o ativo passa o limite superior. Tem os subcampos "subject" e "body" para definir a mensagem padrão.
- ResendEmailTime: define em quanto tempo (em segundos) um email será reenviado caso o ativo continue no mesmo estado. 
- VerificationDelay: define de quanto em quanto tempo (em segundos) acontecerão as verificações.

### Alterando .env
Para o programa rodar, é necessário configurar as variáveis de ambiente como:
- HOST: nome do provedor. Por exemplo, se for gmail fica: "smtp.gmail.com"
- EMAIL: email do remetente.
- PASSWORD: senha do remetente. Nota que essa senha deve ser obtida configurando o email com AppPassword. Siga o [guia](https://support.google.com/accounts/answer/185833?hl=en) para mais informações.

## Instalando
Para instalar o projeto, leve em consideração que este foi feito utilizando Ubuntu 22.04 e 
DotNet 8.0.416. Como requisito do dotnet, faça ```dotnet add package DotNetEnv```.

### Compilando
Para compilar o projeto, utilize ```dotnet build```. O executável (windows) e o binário (linux) estão presentes na pasta build.
