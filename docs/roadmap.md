# Roadmap · Crontrole

[← Índice da documentação](README.md)

O Crontrole usa **Alpha v1.0** como identidade experimental. A base atual inclui o ícone, atalhos combinados por botões, arraste pelo controle e pacote Windows dependente do .NET 10 Desktop Runtime.

## Implementado na base atual

- [x] Leitura de entradas XInput.
- [x] Monitor digital de botões, gatilhos e analógicos.
- [x] Identificação de botão físico por evento.
- [x] Ações de teclado, mouse, mídia e atalhos personalizados.
- [x] Movimentação do cursor e scroll pelos sticks.
- [x] Serviço independente de mouse com intervalo-alvo de 8 ms.
- [x] Persistência local em JSON e compatibilidade com perfis existentes.
- [x] Ações e analógicos habilitados por padrão, com caixas para desativá-los.
- [x] Alt mantido enquanto o botão do controle estiver pressionado; Alt+Tab entre dois botões.
- [x] Clique esquerdo mantido para arrastar com o stick.
- [x] Ícone do aplicativo e botão para remover atalhos salvos.

## Próximos incrementos propostos

### Consolidação pública

- [x] Atualizar título e identidade visual do aplicativo para **Crontrole**.
- [ ] Documentar e testar migração dos identificadores técnicos legados.
- [x] Manter `bin/` e `obj/` fora do versionamento.
- [x] Publicar ZIP executável para Windows x64 que usa o .NET 10 instalado.
- [ ] Publicar release **Pre-release** com changelog e arquivos testados.

### Evoluções posteriores (sem data prometida)

- [ ] Sensibilidade, aceleração e zona morta ajustáveis pelo layout.
- [ ] Múltiplos perfis e alternância rápida.
- [ ] Controle na bandeja do Windows (tray) e inicialização opcional.
- [ ] Macrocurtas, combinações, toque longo e regras de precedência.
- [ ] Seletor de controle quando há mais de um dispositivo XInput.
- [ ] Testes automatizados e build no GitHub Actions.
- [ ] Arquitetura de plugins para ações de terceiros e APIs, incluindo integração opcional com OpenAI.

## Histórico técnico

- [TASK-003 — estrutura inicial](TASK-003.md)
- [TASK-005 — identificação de entradas](TASK-005.md)
- [TASK-006 — captura de entrada e combinações](TASK-006.md)

As tasks acima são registros históricos, não uma afirmação de aceite atualizado.
