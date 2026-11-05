# Publicação de releases

[← Índice da documentação](README.md)

## Identidade e versão

- **Produto:** Crontrole.
- **Versão atual:** Alpha v1.0 experimental.
- **Pacote no repositório:** [`dist/Crontrole-Alpha-v1.0-win-x64.zip`](../dist/Crontrole-Alpha-v1.0-win-x64.zip).
- **Tipo:** *Pre-release*, enquanto o aplicativo estiver experimental.
- **Branch de origem:** `main` após revisão dos arquivos.

O pacote contém `Crontrole.exe` e três arquivos de suporte. Ele depende do **.NET 10 Desktop Runtime** instalado no Windows; não é autossuficiente.

## Criar pacote Windows x64

No Windows, com o .NET 10 SDK instalado e a compilação validada:

```powershell
dotnet publish .\src\Kp4040.App\Kp4040.App.csproj `
  -c Release -p:AssemblyName=Crontrole `
  -o .\dist\win-x64

$files = @('Crontrole.exe', 'Crontrole.dll', 'Crontrole.deps.json', 'Crontrole.runtimeconfig.json') |
  ForEach-Object { Join-Path '.\dist\win-x64' $_ }
Compress-Archive -LiteralPath $files `
  -DestinationPath .\dist\Crontrole-Alpha-v1.0-win-x64.zip -Force
```

O parâmetro `AssemblyName=Crontrole` produz `Crontrole.exe` sem renomear o projeto C# nem a pasta do perfil. Antes de distribuir, inclua no ZIP somente o executável, DLL e dois arquivos JSON necessários.

## Checklist antes de publicar

1. Executar `dotnet build` e testar a janela no Windows 11.
2. Validar botões, identificação, teclas, mouse, scroll e desligamento de ações.
3. Verificar se o perfil JSON antigo continua carregando.
4. Excluir `bin/`/`obj/` do versionamento, conferir `.gitignore` e revisar segredos.
5. Atualizar o README e notas de lançamento conforme as funcionalidades realmente testadas.
6. Anexar o ZIP do executável **somente após testá-lo**.
7. Se criar um novo Release, use uma tag nova apontando para o commit correto e marque **Pre-release**.

A página para publicar é [Releases → New release](https://github.com/SrMahal/crontrole-win/releases/new). A publicação é uma ação separada; preparar a documentação não publica uma versão automaticamente.
