# TASK 003 — Alpha 0.3: base desktop e ações locais

**Estado:** implementação inicial entregue em código-fonte; compilação e aceite pendentes no Windows 11 do usuário.

## Contexto validado nos testes anteriores

- Interface de entrada Windows 11: `Controller (XBOX 360 For Windows)`, XInput funcional.
- 14 botões digitais reconhecidos com códigos padrão XInput.
- LT/RT retornaram valores `0..255`.
- LX/LY e RX/RY retornaram extremos `-32768..32767`.
- Um driver Nefarius existe na máquina, mas sua participação no controle não foi demonstrada. **Não o alterar.**

## Escopo implementado

- [x] Leitura XInput de índice 0 a 3, primeira entrada conectada.
- [x] Monitor de eventos digitais `pressionado`/`solto` sem disparos repetidos por frame.
- [x] Destaque visual de botões, números dos gatilhos e eixos.
- [x] Presets de ações por botão, 14 entradas digitais + 2 gatilhos.
- [x] Actions: Nenhuma, Copiar, Colar, Enter, Selecionar tudo, Desfazer, Abrir ChatGPT.
- [x] Disparo de teclas com Win32 `SendInput`, sem emuladores externos.
- [x] Persistência JSON local com `SchemaVersion = 1`.
- [x] Ações desativadas por padrão a cada inicialização.
- [x] Tratamento básico de conexão/perda de conexão.
- [ ] Compilação efetiva no Windows 11 / .NET 10 (não verificável no ambiente de preparação).
- [ ] Testes de aceitação pelo usuário no KP-4040 real.

## Checklist de aceite no Windows

1. Execute `dotnet run --project .\src\Kp4040.App\Kp4040.App.csproj` e confirme abertura da janela.
2. Conecte e desconecte o KP-4040: estado na UI deve acompanhar.
3. Pressione A, B, X, Y e D-Pad. Apenas a entrada pressionada deve ficar destacada; o último evento deve indicar pressionar/soltar.
4. Movimente os dois analógicos e gatilhos: números devem atualizar.
5. Com ações **desligadas**, confirme que pressionar A/B/X/Y não envia teclas para outros programas.
6. Mude os presets e confirme que `%LOCALAPPDATA%\KP4040AIController\profiles\default.json` aparece e preserva as escolhas após reabrir o app.
7. Com **Bloco de Notas em foco**, habilite ações, teste Ctrl+C/Ctrl+V/Enter; segurando botão, apenas um disparo por pressão.
8. Com a janela do controlador em foco, atalhos de teclado devem ser ignorados para impedir autopreenchimento da própria interface.
9. Confirme que botão Y abre `chatgpt.com` (somente se essa ação estiver configurada e a execução habilitada).
10. Desative ações e confirme que continuam apenas o monitoramento e a edição dos presets.

## Próximas tasks propostas

**TASK 004 — Perfis e UX:** múltiplos perfis, seletor explícito de índice XInput, tray icon, inicialização opcional, atalhos para alternar perfis, detecção de conflito e botão de restauração.

**TASK 005 — Eventos avançados:** toque curto/longo, combinações com precedência, hysteresis dos gatilhos, deadzones calibráveis, mapeamento de movimentos de stick (com cuidado contra drift).

**TASK 006 — OpenAI:** plugin de ações com provedor de IA substituível, interface de prompts e custos, uso opt-in, chave em armazenamento seguro separado dos perfis e política de privacidade clara.

## Critério arquitetural

Separar interface de entrada (XInput), detecção de bordas, perfis e mecanismo de ações. **Não** criar driver kernel nem mexer em drivers existentes. Não introduzir servidor, banco de dados, contas ou API Key na versão inicial.
