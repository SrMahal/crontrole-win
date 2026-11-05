# TASK 006 — Crontrole Alpha v1.0: entrada confiável e combinações manuais

## Problema diagnosticado

Na versão anterior, o `DispatcherTimer` WPF fazia a captura de botões a cada 45 ms. Pressionamentos curtos entre leituras podiam se perder. Além disso, a proteção de foco ignorava o envio de comandos sobre o configurador sem esclarecer o motivo.

## Implementação

- [x] Novo `ControllerPollingService`: leitura XInput fora da UI, alvo de 8 ms, fila limitada de eventos.
- [x] Leitura de snapshots e eventos desacoplada da renderização WPF (45 ms).
- [x] Rejeição de eventos atrasados (mais de 300 ms) após travamento temporário da interface.
- [x] Monitoramento independente do mouse analógico preservado.
- [x] Mensagens claras para ações desativadas, sem atribuição ou bloqueadas por foco.
- [x] Editor manual para atalhos de teclado como `Alt+Tab`, `Ctrl+Shift+S` e `Win+E`.
- [x] `Alt+Tab` permitido mesmo com a janela do Crontrole ativa; demais proteções de foco mantidas.
- [x] Perfil JSON `SchemaVersion = 1` preservado: modelo `KeyboardShortcut` inalterado.

## Testes de aceite no Windows

1. Faça backup de `%LOCALAPPDATA%\KP4040AIController\profiles\default.json`.
2. `dotnet build .\Kp4040.App.slnx` e inicie via `dotnet run --project .\src\Kp4040.App\Kp4040.App.csproj`.
3. Pressione 30 vezes A (toques curtos e longos); confirme registro consistente do evento e da ação.
4. Com o configurador em foco, acione um botão atribuído a Copiar: a mensagem deve explicar que foi ignorado por foco.
5. Atribua B → "Teclado · Gravar tecla/atalho" → "Editar combinação..." → `Alt+Tab`.
6. Habilite ações e pressione B; a troca de janela deve ocorrer, inclusive se Crontrole estiver ativo.
7. Teste `Ctrl+Shift+S` no Bloco de Notas ou editor compatível; confira o JSON salvo após reiniciar.
8. Teste cliques, scroll e cursor analógicos para garantir ausência de regressões.
9. Verifique o comportamento de atalhos direcionados a programas elevados: a política de privilégio do Windows pode bloquear `SendInput`.

## Limitações

- O intervalo de 8 ms é uma meta, não frequência garantida pelo Windows. Eventos menores do que o intervalo real ainda podem se perder.
- O editor cria **combinações simultâneas** de teclado; não implementa macros temporizadas com várias etapas.
- `Ctrl+Alt+Del` e `Win+L` são recusados para evitar comandos de sistema protegidos ou perigosos durante os testes.
- Atalhos globais podem ser interceptados pelo Windows e não funcionar em todos os contextos.
- A aplicação ainda não foi compilada ou executada no ambiente de geração; faça o teste no Windows 11.
