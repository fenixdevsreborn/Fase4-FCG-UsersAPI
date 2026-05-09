# PostgreSQL Local - UsersAPI

## ⚠️ ATENÇÃO: Configuração Apenas para Desenvolvimento Local

Este diretório contém configurações de PostgreSQL **apenas para desenvolvimento local e testes isolados**.

**NÃO use estas configurações em produção ou com o Orchestrator!**

## Quando Usar

- Desenvolvimento local com Kubernetes (namespace `gamestore`)
- Testes isolados do UsersAPI
- Quando você precisa de um ambiente Kubernetes local separado

## Quando NÃO Usar

- ❌ Produção
- ❌ Deploy com o Orchestrator (use `Fase2-Orchestrator/k8s/postgres-users/`)
- ❌ Ambiente compartilhado

## Configuração de Produção

Para produção/orquestração, use:
- `Fase2-Orchestrator/k8s/postgres-users/` (namespace `fiap-gamestore`)

## Diferenças

| Aspecto | Local (este diretório) | Produção (Orchestrator) |
|---------|------------------------|-------------------------|
| Namespace | `gamestore` | `fiap-gamestore` |
| Service Name | `postgres` | `postgres-users-service` |
| Deployment Name | `postgres` | `postgres-users` |
| PVC Name | `postgres-pvc` | `postgres-users-pvc` |

## Arquivos

- `postgres-deployment.yaml` - Deployment do PostgreSQL local
- `postgres-service.yaml` - Service do PostgreSQL local
- `postgres-pvc.yaml` - PersistentVolumeClaim do PostgreSQL local
