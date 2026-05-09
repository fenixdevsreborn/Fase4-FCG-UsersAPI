# UsersAPI

Microsserviço responsável pelo **gerenciamento de usuários** e pela **publicação de eventos de domínio** (UserCreatedEvent) utilizados por outros microsserviços, especialmente para envio de e-mail de boas-vindas. 

## Índice

1. Visão Geral
2. Responsabilidades do Serviço
3. Arquitetura e Tecnologias
4. Comunicação Assíncrona
5. Estrutura do Projeto
6. Endpoints Disponíveis
7. Variáveis de Ambiente
8. Execução (Local, Docker, Kubernetes)
9. Considerações Acadêmicas

---

## 1. Visão Geral

O **UsersAPI** é um microsserviço desenvolvido com arquitetura orientada a eventos para:

* **Cadastrar usuários** no sistema;
* **Autenticar usuários** (em integrações com outros serviços, quando aplicável);
* **Publicar eventos** quando um usuário é criado, acionando microsserviços dependentes (ex.: NotificationsAPI). 

Este serviço faz parte da arquitetura de microsserviços implmentada no projeto da **Fase 2 – Tech Challenge** e é integrado ao ecossistema de serviços via **RabbitMQ/MassTransit**. 

---

## 2. Responsabilidades do Serviço

* Cadastro de novos usuários;
* Emissão de event UserCreatedEvent após cadastro bem-sucedido;
* Gerenciamento das operações necessárias para a autenticação/autorização do usuário;
* Publicação de eventos para consumo por outros microsserviços (ex.: NotificationsAPI). 

---

## 3. Arquitetura e Tecnologias

**Plataforma e linguagem:**

* .NET 9.0
* C#

**Padrões e ferramentas:**

* Microsserviços orientados a eventos
* Mensageria com **RabbitMQ**
* **MassTransit** para abstração de mensageria
* **PostgreSQL** como datastore
* **Docker** para containerização
* **Kubernetes** para orquestração (manifestos incluídos) 

---

## 4. Comunicação Assíncrona

A UsersAPI publica eventos assíncronos para que outros microsserviços reajam com base nas mudanças de estado dos usuários. O fluxo principal é:

1. Um novo usuário é cadastrado via endpoint da API;
2. A API publica um evento de domínio chamado **UserCreatedEvent** no broker;
3. Outros serviços (ex.: **NotificationsAPI**) consomem este evento para realizar ações (como envio de e-mail de boas-vindas). 

---

## 5. Estrutura do Projeto

Estrutura típica do repositório:

````
UsersAPI
├── src
│   ├── UsersAPI
│   │   ├── Controllers
│   │   ├── Application
│   │   ├── Domain
│   │   ├── Infrastructure
│   │   └── Program.cs
├── k8s
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── configmap.yaml
│   └── secret.yaml
├── Dockerfile
├── docker-compose.yml
└── README.md
````

---

## 6. Endpoints Disponíveis

> **Observação:** Os nomes exatos dos endpoints dependem da implementação interna (Controllers). Abaixo estão os principais esperados para cadastro e consulta de usuário.

| Verbo HTTP | Endpoint | Autenticação | Descrição |
|------------|----------|--------------|-----------|
| POST | `/api/users` | Não | Cadastrar novo usuário |
| GET | `/api/users/{id}` | Sim (quando aplicável) | Consultar dados do usuário |
| POST | `/api/auth/login` | Não | Autenticar usuário (token JWT) |
| GET | Auto-documentação /health | Não | Health check |
| GET | Swagger UI | Não | Documentação interativa |

*(Adapte estes endpoints conforme implementação real de controllers no projeto)*

---

## 7. Variáveis de Ambiente

Configure as variáveis abaixo via **ConfigMap** e **Secrets** no Kubernetes ou via ambiente local:

**ConfigMap (não sensíveis):**
- `RABBITMQ_HOST`
- `RABBITMQ_QUEUE_USER_CREATED`
- `JWT_ISSUER`
- `JWT_AUDIENCE`

**Secrets (sensíveis):**
- `POSTGRES_CONNECTION_STRING`
- `JWT_SECRET_KEY`

**Observações:**
- O `POSTGRES_CONNECTION_STRING` deve apontar para a instância de PostgreSQL utilizada pela API.
- `JWT_SECRET_KEY` é utilizado para assinatura/validação de tokens JWT.

---

## 8. Execução

### 8.1 Locally (Desenvolvimento)

1. Clone o repositório:
   ```bash
   git clone https://github.com/thefenixdevs/Fase2-UsersAPI.git


2. Configure as variáveis de ambiente no seu ambiente de desenvolvimento.
3. Execute via .NET:

   ```bash
   dotnet restore
   dotnet build
   dotnet run --project src/UsersAPI/UsersAPI.csproj
   ```

### 8.2 Docker

1. Construa a imagem:

   ```bash
   docker build -t users-api .
   ```
2. Execute o container:

   ```bash
   docker run -p 5000:8080 users-api
   ```
3. Quando integrado ao ambiente completo:

   ```bash
   docker-compose up
   ```

### 8.3 Kubernetes

Certifique-se de que o Kubernetes do seu ambiente (ex.: Docker Desktop) esteja habilitado. Aplique os manifests:

```bash
kubectl apply -f k8s/
```

Verifique os pods em execução:

```bash
kubectl get pods
```

---

## 9. Considerações Acadêmicas

Este microsserviço foi desenvolvido com foco educacional e abrange de forma completa os principais padrões esperados na certificação da **Fase 2** do desafio FIAP:

* **Orientação a eventos e comunicação assíncrona**;
* **Mensageria com RabbitMQ e MassTransit**;
* **Containerização e orquestração com Kubernetes**;
* **Separação de responsabilidades e arquitetura modular**. 

---
