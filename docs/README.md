# Documentação · Crontrole

Este diretório reúne as instruções de uso e os detalhes técnicos. O [README principal](../README.md) permanece como apresentação rápida do projeto.

## Para quem vai usar

- **[Instalação](instalacao.md)** — requisitos, obtenção do código e execução no Windows 11.
- **[Guia de uso](uso.md)** — reconhecimento do controle, identificação de botões, atalhos, mouse e perfil local.
- **[Segurança e privacidade](seguranca.md)** — permissões, limites da entrada sintética e armazenamento de configurações.

## Para quem vai desenvolver

- **[Arquitetura](arquitetura.md)** — componentes reais e fluxo de eventos XInput → ações.
- **[Desenvolvimento](desenvolvimento.md)** — comandos de build, execução e pontos de extensão.
- **[Roadmap](roadmap.md)** — estado validado e propostas para próximas versões.
- **[Publicação de releases](publicacao.md)** — empacotamento Windows x64 e checklist de pré-lançamento.

## Histórico de tarefas

Estes documentos foram preservados como registros de implementação e podem conter checklists históricos ainda não atualizados:

- [TASK-003 — Base desktop e ações locais](TASK-003.md)
- [TASK-005 — Identificação física e ações de entrada](TASK-005.md)

> **Nomenclatura:** a marca pública agora é **Crontrole**, com **Alpha v1.0** como nome planejado para o primeiro lançamento. O projeto C# mantém, no momento, o identificador interno `Kp4040.App`; a base técnica publicada foi identificada como Alpha 0.5.1. Renomear o projeto sem migrar cuidadosamente namespaces, recursos e perfis pode quebrar a aplicação.
