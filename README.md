# Fase4-FCG-UsersAPI

Microsserviço responsável pelo **gerenciamento de usuários** e pela **publicação de eventos de domínio** (`UserCreatedEvent`) utilizados por outros microsserviços, especialmente para envio de e-mail de boas-vindas.

> **Branch alvo da pipeline:** `master`.
> **Registries:** AWS ECR (`users-api`) **e** Docker Hub (`<DOCKERHUB_USERNAME>/fcg-users-api`).
> Pipeline: [`.github/workflows/users-api-ci-cd.yml`](.github/workflows/users-api-ci-cd.yml).

---

## ⚙️ Configuração obrigatória para automação AWS + Docker Hub

> **A documentação master da configuração manual está em [`Fase4-FCG-Orchestrator/docs/MANUAL-STEPS.md`](../Fase4-FCG-Orchestrator/docs/MANUAL-STEPS.md).** Esta seção é o resumo específico desta API.

### 1. Pré-requisitos (uma vez na organização)

- Bootstrap AWS executado em `Fase4-FCG-Orchestrator/infra/terraform/bootstrap/` (cria OIDC + IAM role)
- Repositório Docker Hub `<DOCKERHUB_USERNAME>/fcg-users-api` criado em hub.docker.com
- Personal Access Token Docker Hub com permissão **Read & Write**
- Personal Access Token GitHub com `contents:write` no repo `Fase4-FCG-Orchestrator` (para o passo de GitOps)
- Repositório ECR `users-api` (criado pelo Terraform principal)

### 2. Branch padrão

A pipeline só dispara em `push` para a branch **`master`**. Configure em **Settings → Branches → Default branch**.

### 3. Secrets e Variables do repositório (Settings → Secrets and variables → Actions)

| Tipo | Nome | Descrição |
|------|------|-----------|
| Secret | `AWS_GITHUB_ROLE_ARN` | ARN da role IAM (output `github_actions_role_arn` do bootstrap Terraform) |
| Secret | `DOCKERHUB_USERNAME` | Username Docker Hub |
| Secret | `DOCKERHUB_TOKEN` | PAT Docker Hub Read & Write |
| Secret | `GITOPS_TOKEN` | PAT GitHub com `contents:write` no `Fase4-FCG-Orchestrator` |
| Variable | `GITOPS_REPOSITORY` | `<seu-org>/Fase4-FCG-Orchestrator` |

Setup rápido com `gh` CLI:

```powershell
$ORG="seu-org"; $REPO="Fase4-FCG-UsersAPI"
gh secret   set AWS_GITHUB_ROLE_ARN --body "<role-arn>"     --repo "$ORG/$REPO"
gh secret   set DOCKERHUB_USERNAME  --body "<dh-user>"      --repo "$ORG/$REPO"
gh secret   set DOCKERHUB_TOKEN     --body "<dh-pat>"       --repo "$ORG/$REPO"
gh secret   set GITOPS_TOKEN        --body "<gh-pat>"       --repo "$ORG/$REPO"
gh variable set GITOPS_REPOSITORY   --body "$ORG/Fase4-FCG-Orchestrator" --repo "$ORG/$REPO"
```

### 4. O que a pipeline faz a cada push em `master`

1. `dotnet restore` + `dotnet build` + `dotnet test` (com PostgreSQL service container)
2. Auditoria NuGet — falha em High/Critical
3. OIDC login em AWS → ECR login → `docker build` → `docker push` em ECR (tag `${GITHUB_SHA::12}`)
4. Login em Docker Hub → `docker tag` + `docker push` em `<DOCKERHUB_USERNAME>/fcg-users-api:<sha>` **e** `:latest`
5. Trivy scan na imagem (falha em High/Critical fixáveis)
6. Checkout do `Fase4-FCG-Orchestrator` (com `GITOPS_TOKEN`) → atualiza tag em `deploy/helm/fcg-platform/values-prod.yaml` → commit & push em `master`
7. Argo CD detecta a mudança e faz rolling update

### 5. Primeiro disparo manual (após criar tudo)

```powershell
gh workflow run users-api-ci-cd.yml --repo "$ORG/Fase4-FCG-UsersAPI" --ref master
```

---


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

Este serviço faz parte da arquitetura de microsserviços implmentada no projeto da **Fase 4 – Tech Challenge** e é integrado ao ecossistema de serviços via **RabbitMQ/MassTransit**. 

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
   git clone https://github.com/<seu-org>/Fase4-FCG-UsersAPI.git


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

Este microsserviço foi desenvolvido com foco educacional e abrange de forma completa os principais padrões esperados na certificação da **Fase 4** do desafio FIAP:

* **Orientação a eventos e comunicação assíncrona**;
* **Mensageria com RabbitMQ e MassTransit**;
* **Containerização e orquestração com Kubernetes**;
* **Separação de responsabilidades e arquitetura modular**. 

---
