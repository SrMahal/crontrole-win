# Roadmap · Crontrole

[← Índice da documentação](README.md)

O Crontrole passa a adotar **Alpha v1.0** como identidade do primeiro lançamento público experimental. A base de código atualmente presente no repositório corresponde à **Alpha 0.5.1**. A reorganização da documentação **não** altera a versão binária nem confirma que o Release já foi publicado.

## Implementado na base atual

- [x] Leitura de entradas XInput.
- [x] Monitor digital de botões, gatilhos e analógicos.
- [x] Identificação de botão físico por evento.
- [x] Ações de teclado, mouse, mídia e atalhos personalizados.
- [x] Movimentação do cursor e scroll pelos sticks.
- [x] Serviço independente de mouse com intervalo-alvo de 8 ms.
- [x] Persistência local em JSON e compatibilidade com perfis existentes.
- [x] Ações desabilitadas por padrão a cada inicialização.

## Próximos incrementos propostos

### Alpha v1.0 — consolidação pública

- [ ] Atualizar título e identidade visual do aplicativo para **Crontrole**.
- [ ] Documentar e testar migração dos identificadores técnicos legados.
- [ ] Corrigir versionamento de `bin/` e `obj/`.
- [ ] Revisar instalador/pacote executável do Windows.
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

As tasks acima são registros históricos, não uma afirmação de aceite atualizado.
