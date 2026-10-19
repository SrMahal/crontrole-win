# 🎮 KP-4040 AI Controller

Aplicativo desktop experimental para transformar o controle USB KNUP KP-4040 em um painel de atalhos no Windows 11. **Não oficial, sem associação à KNUP, Microsoft ou OpenAI.**

**Alpha 0.5 — código-fonte WPF/C#/.NET 10**. Lê um controle XInput, mostra os estados em tempo real e permite associar botões a atalhos simples do Windows. Não requer API Key, driver customizado, NuGet extra nem permissões de administrador.

## Iniciar no Windows 11

1. Instale o **.NET 10 SDK (Windows)** a partir de https://dotnet.microsoft.com/download/dotnet/10.0, caso ainda não tenha instalado. O SDK é necessário para compilar o projeto.
2. Extraia o ZIP em uma pasta, por exemplo `C:\Projetos\kp4040-ai-controller`.
3. Conecte o KP-4040 e confirme que `joy.cpl` o identifica como um controle Xbox 360, como nos testes anteriores.
4. Abra o PowerShell na pasta raiz do projeto e execute:

```powershell
dotnet --version
dotnet run --project .\src\Kp4040.App\Kp4040.App.csproj
```

Também é possível abrir `src/Kp4040.App/Kp4040.App.csproj` em uma versão do Visual Studio compatível com .NET 10 e WPF.

> O ZIP é **código-fonte**, não um binário compilado. A compilação e o teste funcional precisam ser feitos no Windows 11. O ambiente usado para preparar o ZIP não disponibiliza o SDK .NET nem a execução WPF.

## Como testar com segurança

- O painel mostra a conexão, os botões pressionados (destaque verde), LT/RT e ambos os sticks.
- Existem presets de exemplo: **A = Ctrl+C; B = Ctrl+V; X = Enter; Y = abrir ChatGPT no navegador**. Mude as ações pelas caixas de seleção. Todas as outras entradas começam em **Nenhuma**.
- **A execução dos atalhos inicia DESLIGADA em toda abertura do aplicativo.** Marque `Habilitar execução de ações` apenas quando desejar testar.
- Para testar atalhos de teclado, abra um Bloco de Notas, selecione um texto e mantenha essa janela em foco. O aplicativo evita enviar atalhos de teclado para sua própria janela ativa.
- Cada pressionamento gera somente **uma ação**, mesmo se o botão ficar pressionado. Um segundo acionamento exige soltar o botão antes.
- Gatilhos LT/RT ficam disponíveis como botões virtuais com limiar fixo de **180/255**. Os eixos LX/LY/RX/RY são somente monitorados nesta versão.
- O projeto usa o **primeiro controle XInput encontrado (índice 0 a 3)**. Se houver vários controles, a seleção manual de dispositivo será um incremento futuro.
- Clique em `Salvar perfil` ou altere qualquer ação para persistir o JSON local. O aplicativo não executa atalhos ao ser iniciado até que você os habilite manualmente.

## Perfil local

`%LOCALAPPDATA%\KP4040AIController\profiles\default.json`

Exemplo (arquivo salvo pelo aplicativo):

```json
{
  "SchemaVersion": 1,
  "Name": "Padrao",
  "TriggerThreshold": 180,
  "Mappings": {
    "A": "Copy",
    "B": "Paste",
    "X": "Enter",
    "Y": "OpenChatGpt",
    "DPadUp": "None"
  }
}
```

As demais teclas também serão incluídas automaticamente. As ações são strings para permitir futuras evoluções de esquema. **Não armazene tokens ou API Keys nesse JSON**.

## Arquitetura

```text
kp4040-ai-controller/
├── README.md
├── Kp4040.App.slnx
├── docs/
│   └── TASK-003.md
├── scripts/
│   └── run.ps1
└── src/
    └── Kp4040.App/
        ├── Kp4040.App.csproj
        ├── App.xaml / App.xaml.cs
        ├── MainWindow.xaml / MainWindow.xaml.cs
        ├── Models/
        │   ├── ActionKind.cs
        │   ├── ButtonMappingRow.cs
        │   ├── ControllerProfile.cs
        │   └── ControllerSnapshot.cs
        └── Services/
            ├── XInputReader.cs     # Entrada de hardware (somente leitura)
            ├── InputMonitor.cs     # Bordas de pressionar/soltar
            ├── ActionExecutor.cs   # Saidas: SendInput/URL
            └── ProfileStore.cs     # JSON local, com versao de esquema
```

### Organização para crescer sem reescrever

- `XInputReader`: substituível no futuro por outro adaptador de entrada (HID/GameInput).
- `InputMonitor`: normaliza entradas do controle em eventos digitais, isolado de ações.
- `ActionExecutor`: mecanismo de saída independente do XInput. No futuro, criar `IActionHandler` por ação, com handlers para comandos, perfis, integração OpenAI etc.
- `ProfileStore`: persistência simples em JSON com `SchemaVersion`; em versões futuras pode evoluir para múltiplos perfis e migração de dados.
- `MainWindow`: apresenta o monitor e edita o mapeamento. Para um projeto maior, extrair `ViewModels/` sem mudar os adaptadores existentes.

## Limites da versão

- Sem API OpenAI, comandos de linguagem natural, execução automática na inicialização, ícone de bandeja, captura de combinações ou editor de macros.
- Sem bloqueio/supressão de eventos originais do gamepad (jogos também poderão receber os botões).
- Sem remapeamento em nível de driver; o aplicativo executa ações **além** dos sinais que o Windows já recebe.
- `SendInput` respeita as restrições de segurança e privilégio do Windows. Aplicações executadas como administrador podem não aceitar teclas sintéticas enviadas por uma aplicação comum.
- Os presets usam nomes lógicos **XInput**. A identidade do equipamento não é garantida pelo VID/PID apresentado no Windows, e o código não identifica por marca ou número de série.

## Publicar um executável (no Windows 11)

Com .NET 10 SDK instalado, na pasta raiz:

```powershell
dotnet publish .\src\Kp4040.App\Kp4040.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

O executável gerado ficará dentro de `src\Kp4040.App\bin\Release\net10.0-windows\win-x64\publish\`. Isso produz um pacote maior, mas dispensa a instalação separada do runtime .NET para os usuários finais. Teste e assine o binário antes de qualquer distribuição pública.

## Segurança e privacidade

- Não exige API Key nem envia informações para servidores próprios.
- **Abrir ChatGPT** apenas abre `https://chatgpt.com/` no navegador; não automatiza nem acessa suas conversas.
- Os presets podem alterar documentos ou aplicativos abertos (por exemplo, colar texto/Enter). Habilite conscientemente.
- Para uma futura integração OpenAI, usar armazenamento de segredos separado dos perfis exportáveis, com configuração explícita do usuário e tratamento de custos/limites.


## Alpha 0.5 — Identificar botão, mouse e teclado

- Clique em **Identificar botão** e pressione A/B/X/Y, D-pad, L1/R1, L3/R3 ou L2/R2 (acima do limiar configurado). A linha reconhecida ficará destacada e visível. Clique novamente para cancelar.
- Selecione uma ação da lista da linha: teclado (presets ou **Gravar tecla/atalho**), mouse (clique, duplo clique, meio, voltar/avançar, scroll 4 direções) ou controles de mídia.
- Em **Gravar tecla/atalho**, clique no botão **Gravar atalho...** e pressione uma tecla, com Ctrl/Shift/Alt/Win se desejar. Esc sozinho cancela a gravação; escolha o preset **Esc** se precisar dessa tecla. Atalhos reservados pelo Windows podem não ser capturados.
- Os atalhos configurados ficam no mesmo `%LOCALAPPDATA%\KP4040AIController\profiles\default.json`, em `Shortcuts`. O JSON anterior da Alpha 0.4 continua válido e é preservado.
- Ao identificar um botão ou gravar um atalho, as ações são suspensas para evitar disparos; com a janela ativa, injeções locais também são bloqueadas. **Ative as ações manualmente** para testar em outro aplicativo.
- O mouse analógico da Alpha 0.4 permanece com seu próprio checkbox. O novo motor **não** instala drivers nem intercepta o controle nos jogos.
- Ao trocar a ação de um botão, o atalho gravado permanece guardado e pode ser reutilizado se você selecionar novamente **Gravar tecla/atalho**.

**Limitações:** apenas um botão XInput por vez na identificação; movimentos dos analógicos não são capturados como botões. O editor de atalhos executa combinações usuais de teclas e algumas teclas especiais (via presets); não é um gravador de macros nem um mapeador global de todos os dispositivos HID.
