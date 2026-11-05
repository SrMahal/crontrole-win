# Desenvolvimento

[← Índice da documentação](README.md)

## Ferramentas

- Windows 11, PowerShell e **.NET 10 SDK**.
- VS Code com extensão C# (opcional) ou Visual Studio compatível com WPF/.NET 10.
- Git para versionamento.

## Executar e compilar

Na raiz do projeto:

```powershell
dotnet --version
dotnet restore .\Kp4040.App.slnx
dotnet build .\Kp4040.App.slnx
dotnet run --project .\src\Kp4040.App\Kp4040.App.csproj
```

Alternativamente, `powershell -ExecutionPolicy Bypass -File .\scripts\run.ps1` executa o script existente, após você verificar seu conteúdo.

## Estrutura resumida

```text
crontrole-win/
├── README.md
├── Kp4040.App.slnx
├── docs/
├── scripts/run.ps1
└── src/Kp4040.App/
    ├── App.xaml + App.xaml.cs
    ├── MainWindow.xaml + MainWindow.xaml.cs
    ├── Models/
    └── Services/
```

O nome `Kp4040.App` é **legado**. A atualização da identidade visual para Crontrole **não renomeia automaticamente** assembly, namespace, `.csproj`, `.slnx` ou o diretório do perfil.

## Onde alterar cada comportamento

- **Layout/UX:** `MainWindow.xaml`.
- **Fluxo de identificação e gravação:** `MainWindow.xaml.cs`.
- **Catálogo de comandos:** `Models/ActionKind.cs`.
- **Execução de ações:** `Services/ActionExecutor.cs`.
- **Sensibilidade e zona morta:** `Services/AnalogMouseController.cs` (constantes atuais).
- **Frequência de leitura do mouse:** `Services/MousePollingService.cs`.
- **Frequência de leitura dos botões:** `Services/ControllerPollingService.cs`.
- **Formato e gravação de perfis:** `Models/ControllerProfile.cs` e `Services/ProfileStore.cs`.

Consulte a [arquitetura detalhada](arquitetura.md).

## Checklist antes de abrir um PR

1. Compile no Windows com `dotnet build`.
2. Confirme que a interface abre com ações e analógicos habilitados e que ambas as caixas podem ser desmarcadas.
3. Teste a detecção de conexão/desconexão e a captura de botões.
4. Teste teclado e mouse com outra janela em foco.
5. Valide scroll, zona morta e desativação imediata do mouse.
6. Verifique compatibilidade de `default.json` anterior e não exponha dados pessoais.
7. **Não inclua `bin/` nem `obj/` no commit.**

## Git e arquivos gerados

A regra `.gitignore` evita novos arquivos gerados. Os antigos arquivos de `bin/` e `obj/` foram removidos do índice do Git; o ZIP distribuível fica em `dist/`.

## Melhorias de código previstas

- Testes automatizados de regras e perfis.
- Workflow de build Windows no GitHub Actions.
- Editor visual de sensibilidade e perfis múltiplos.
- Renomeação técnica controlada da marca, sem perder os perfis existentes.
