#Async Communication API

API para agendamento e envio assíncrono de comunicações (e-mail, SMS, push, WhatsApp), construída com .NET 10, RabbitMQ e MySQL.

---

## Arquitetura

```
+-----------------+     +-------------+     +-----------------+
¦  Communication  ¦----?¦   RabbitMQ  ¦----?¦  Communication  ¦
¦      API        ¦     ¦    Queue    ¦     ¦     Worker      ¦
+-----------------+     +-------------+     +-----------------+
        ¦                                            ¦
        ?                                            ?
  +-----------+                              +---------------+
  ¦   MySQL   ¦                              ¦  Gmail SMTP   ¦
  +-----------+                              +---------------+
```

O projeto segue os princípios de **Clean Architecture** dividido em camadas:

| Projeto | Responsabilidade |
|---|---|
| `Communication.Domain` | Entidades, enums e interfaces |
| `Communication.Application` | Casos de uso, DTOs, eventos |
| `Communication.Infrastructure` | EF Core, repositórios, RabbitMQ |
| `Communication.Api` | Controllers REST + Swagger |
| `Communication.Worker` | Consumidor da fila + envio de e-mail |

---

## Tecnologias

- **.NET 10**
- **ASP.NET Core** — API REST
- **Entity Framework Core 9** + **Pomelo** — ORM para MySQL
- **MySQL 8** — banco de dados
- **RabbitMQ** — mensageria assíncrona
- **MailKit** — envio de e-mail via SMTP
- **Docker + Docker Compose** — containerização
- **Swagger** — documentação da API

---

## Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

---

##  Como rodar

### 1. Clone o repositório
```bash
git clone https://github.com/GabrielPassarinVicente/async-communication-api.git
cd async-communication-api
```

### 2. Configure as variáveis de ambiente

Edite o `docker-compose.yml` com suas credenciais de e-mail:

```yaml
Email__Username: seu@gmail.com
Email__Password: sua-senha-de-app
```

> Para Gmail, gere uma **senha de app** em: `Conta Google ? Segurança ? Senhas de app`

### 3. Suba os containers
```bash
docker compose up --build -d
```

### 4. Acesse a documentação
- **Swagger**: [http://localhost:8081/swagger](http://localhost:8081/swagger)
- **RabbitMQ Management**: [http://localhost:15673](http://localhost:15673) — `guest` / `guest`

---

## Endpoints

### POST `/api/scheduling` — Agendar comunicação
```json
{
  "scheduleDate": "2025-06-01T10:00:00",
  "recipient": "destinatario@email.com",
  "message": "Olá! Esta é uma mensagem agendada.",
  "type": 0
}
```

| Tipo | Valor |
|---|---|
| Email | 0 |
| SMS | 1 |
| Push | 2 |
| WhatsApp | 3 |

**Resposta:** `201 Created`
```json
{ "id": "uuid-do-agendamento" }
```

---

### GET `/api/scheduling/{id}` — Consultar status

**Resposta:** `200 OK`
```json
{ "scheduling": "Pending" }
```

| Status | Descrição |
|---|---|
| `Pending` | Aguardando processamento |
| `Sent` | Enviado com sucesso |
| `Cancelled` | Cancelado |

---

### PATCH `/api/scheduling/{id}/cancel` — Cancelar agendamento

**Resposta:** `204 No Content`

---

## Fluxo de funcionamento

1. Cliente faz `POST /api/scheduling`
2. API salva o agendamento no MySQL com status `Pending`
3. API publica um evento na fila `communication-requests` do RabbitMQ
4. Worker consome a mensagem da fila
5. Worker envia o e-mail via MailKit (SMTP Gmail)
6. Worker atualiza o status para `Sent` no banco

---

## Serviços Docker

| Serviço | Porta Host | Descrição |
|---|---|---|
| `communication-api` | `8081` | API REST |
| `communication-worker` | — | Worker de processamento |
| `communication-mysql` | `3307` | Banco de dados |
| `communication-rabbitmq` | `5673` / `15673` | Message broker |
