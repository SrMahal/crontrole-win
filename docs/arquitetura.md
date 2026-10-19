# Arquitetura

[← Índice da documentação](README.md)

## Princípios

O Crontrole é um **aplicativo desktop local**, não um driver nem um serviço web. Separamos leitura do controle, interpretação de eventos, execução de comandos e persistência em serviços independentes. A interface WPF organiza a configuração e o monitoramento.

```text
Controle físico XInput
       |
       +--> XInputReader --> ControllerSnapshot
                |                  |
                |                  +--> InputMonitor --> ButtonEvent
                |                                      |
                |                                      +--> MainWindow --> ActionExecutor --> SendInput / navegador
                |
                +--> MousePollingService (thread própria, alvo 8 ms)
                                          |
                                          +--> AnalogMouseController --> SendInput (mouse)

ControllerProfile <--> ProfileStore <--> JSON em %LOCALAPPDATA%
              |
              +--> MainWindow (configurações WPF)
```

## Módulos e responsabilidades

| Caminho real no repositório | Responsabilidade |
| --- | --- |
| [`src/Kp4040.App/MainWindow.xaml`](../src/Kp4040.App/MainWindow.xaml) | Interface e controles WPF. |
| [`src/Kp4040.App/MainWindow.xaml.cs`](../src/Kp4040.App/MainWindow.xaml.cs) | Coordenação entre UI, mapeamentos e serviços. |
| [`Services/XInputReader.cs`](../src/Kp4040.App/Services/XInputReader.cs) | Lê o primeiro controle XInput conectado. |
| [`Services/InputMonitor.cs`](../src/Kp4040.App/Services/InputMonitor.cs) | Converte mudanças em eventos de pressionar/soltar. |
| [`Services/ActionExecutor.cs`](../src/Kp4040.App/Services/ActionExecutor.cs) | Executa atalhos, ações de mouse/mídia e abertura de URL. |
| [`Services/MousePollingService.cs`](../src/Kp4040.App/Services/MousePollingService.cs) | Leitura rápida independente da interface para analógicos. |
| [`Services/AnalogMouseController.cs`](../src/Kp4040.App/Services/AnalogMouseController.cs) | Aplica zona morta e converte analógicos em mouse/scroll. |
| [`Services/ProfileStore.cs`](../src/Kp4040.App/Services/ProfileStore.cs) | Serialização e salvamento local do perfil JSON. |
| [`Models/ControllerSnapshot.cs`](../src/Kp4040.App/Models/ControllerSnapshot.cs) | Estado dos botões, eixos e gatilhos. |
| [`Models/ActionKind.cs`](../src/Kp4040.App/Models/ActionKind.cs) | Catálogo de tipos de ação disponíveis. |
| [`Models/ControllerProfile.cs`](../src/Kp4040.App/Models/ControllerProfile.cs) | Dados e esquema do perfil. |
| [`Models/ButtonMappingRow.cs`](../src/Kp4040.App/Models/ButtonMappingRow.cs) | Estado editável de cada linha da UI. |
| [`Models/KeyboardShortcut.cs`](../src/Kp4040.App/Models/KeyboardShortcut.cs) | Estrutura dos atalhos personalizados. |

## Ciclo de atualização

O `MainWindow` usa um `DispatcherTimer` de **45 ms** para atualizar a interface, estados digitais e execução de ações vinculadas a botões. O `MousePollingService` utiliza uma thread própria com `Thread.Sleep(8)` como **alvo** para suavizar cursor e scroll. A precisão real não é garantida pelo scheduler do Windows.

O `InputMonitor` detecta mudanças de estado e evita disparar novamente uma ação a cada frame enquanto um botão está segurado.

## Persistência

`ControllerProfile` usa `SchemaVersion = 1`, com `Mappings` e `Shortcuts`. `ProfileStore` salva usando arquivo temporário e substituição posterior. A evolução para múltiplos perfis precisa manter compatibilidade e considerar migrações de dados.

## Pontos de extensão planejados

- Interface de adaptadores para mais APIs de entrada (além de XInput).
- Executores por tipo de ação, isolando teclado, mouse, perfis, macros e integrações externas.
- Separação progressiva de `ViewModels` quando a UI crescer.
- Provider opcional da OpenAI, com gestão explícita de credenciais, custo e permissões.

Essas extensões **não estão implementadas** na base publicada.
